using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// A facade which evaluates conductor attributes.
    /// </summary>
    public class ConductorAttributeFacade
    {

        #region private
        private const string _cancelEditError = "{0} OID: {1}:  The Number of Phases value does not match the sum of the Conductor Counts from the Conductor Info records.";
        private const string _cancelPhaseError = "The Phase Designation of Conductor Info records is not valid for domain.";
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static List<string> _validationErr = null;
        #region helper methods
        ///// <summary>
        ///// validates value against field domain.
        ///// </summary>
        ///// <param name="fld">The Field.</param>
        ///// <param name="val">The Value.</param>
        ///// <returns></returns>
        //private bool IsDomainValueValid(IField fld, int val)
        //{
        //    //get the domain assigned to the field
        //    _logger.Debug("Checking for valid value for domain.");
        //    bool isValid = false;
        //    IDomain dmain = fld.Domain;
        //    //check if codedvalue domain
        //    if (dmain is ICodedValueDomain)
        //    {
        //        //check value is valid or not
        //        isValid = dmain.MemberOf(val);
        //    }

        //    _logger.Debug(string.Format("Value is {0} for domain.", isValid ? "valid" : "invalid"));
        //    return isValid;
        //}

        /// <summary>
        /// validates value against field domain.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="fldIndex">The filed index.</param>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        private bool IsDomainValueValid(IFeature feature, int fldIndex, int val)
        {
            IField fld = feature.Fields.Field[fldIndex];
            //get the domain assigned to the field
            _logger.Debug("Checking for valid value for domain.");
            bool isValid = false;
            IDomain dmain = fld.Domain;
            if (dmain == null)
            {
                dmain = ((ISubtypes)feature.Table).HasSubtype
                        ? ((ISubtypes)feature.Table).get_Domain(((IRowSubtypes)feature).SubtypeCode, fld.Name)
                        : fld.Domain;
            }
            //check if codedvalue domain
            if (dmain is ICodedValueDomain)
            {
                //check value is valid or not
                isValid = dmain.MemberOf(val);
            }

            _logger.Debug(string.Format("Value is {0} for domain.", isValid ? "valid" : "invalid"));
            return isValid;
        }

        /// <summary>
        /// Returns True if passed ObjClass do have Modelname "PGE_PRIMARYCONDUCTOR".
        /// </summary>
        /// <param name="objClass">The ObjectClass to be checked.</param>
        /// <returns>true if Modelname contains "PGE_PRIMARYCONDUCTOR"</returns>
        private bool IsPrimary(IObjectClass objClass)
        {
            //check if PrimaryConductor model name is assigned to this object class
            return ModelNameFacade.ContainsClassModelName(objClass, new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryConductor });
        }

        /// <summary>
        /// Add Error message to the List.
        /// </summary>
        /// <param name="errorMessage">The Message.</param>
        private void AddError(string errorMessage)
        {
            //add error message in ID8List
            _logger.Debug(string.Format("Adding Message: {0}", errorMessage));
            if (_validationErr == null)
            {
                //if instance is not created then create
                _validationErr = new List<string>();
            }
            _validationErr.Add(errorMessage);
            _logger.Debug(string.Format("Added Message: {0}", errorMessage));
        }

        /// <summary>
        /// Returns the count of 
        /// </summary>
        /// <param name="isPrimary">True if PrimaryConductor class model name present in current FeatureClass.</param>
        /// <param name="destSet">All related objects to the current FeatureClass.</param>
        /// <param name="destRow">The Object to be created/deleted or updated.</param>
        /// <param name="isDelete">True if delete event.</param>
        /// <returns>PhaseCount of ConductorInfo object class.</returns>
        private int GetInfoPhaseCount(bool isPrimary, ESRI.ArcGIS.esriSystem.ISet destSet)
        {
            destSet.Reset();
            int CEDSANumberofPhases = 0;
            IRow unitRow = destSet.Next() as IRow;
            if (unitRow == null)
            {
                //destSet is empty, returning 0
                _logger.Debug("both destRow and destSet are <Null>, returning 0.");
                return 0;
            }
            //cast Table to Objectclass
            IObjectClass unitFeatureClass = unitRow.Table as IObjectClass;

            //get the indexes from fieldmodel name
            int countIdx = ModelNameFacade.FieldIndexFromModelName(unitFeatureClass, SchemaInfo.Electric.FieldModelNames.CondutorCount);
            int useIdx = ModelNameFacade.FieldIndexFromModelName(unitFeatureClass, SchemaInfo.Electric.FieldModelNames.CondutorUse);

            //check if index is -1(not found)
            if (countIdx == -1)
            {
                _logger.Warn(SchemaInfo.Electric.FieldModelNames.CondutorCount + "-FieldModelname is not assigned for objectclass " + unitFeatureClass.AliasName + ", exiting.");
                return -1;
            }
            else if (useIdx == -1)
            {
                _logger.Warn(SchemaInfo.Electric.FieldModelNames.CondutorUse + "-FieldModelname is not assigned for objectclass " + unitFeatureClass.AliasName + ", exiting.");
                return -1;
            }

            //iterate all rows from set
            _logger.Debug("Counting number of phases.");
            do
            {
                int useValue = 0;
                int currentCount = 0;
                //parse count value to int
                int.TryParse(unitRow.get_Value(countIdx).ToString(), out currentCount);

                //check if primary then we need to consider ConductorUse
                if (isPrimary)
                {
                    int.TryParse(unitRow.get_Value(useIdx).ToString(), out useValue);
                    if (useValue != 1)
                    {
                        continue;
                    }
                }
                //sum up the conductor count value
                CEDSANumberofPhases += currentCount;
            }
            while ((unitRow = destSet.Next() as IRow) != null);
            _logger.Debug("Completed counting number of phases(" + CEDSANumberofPhases + ").");
            //return the sum up count
            return CEDSANumberofPhases;
        }

        /// <summary>
        /// Returns new phasedesignation value.
        /// </summary>
        /// <param name="origDesig">the original phasedesignations value.</param>
        /// <param name="destDesig">the phasedesignation value being created.</param>
        /// <returns>new phasedesignation value based on original and created values.</returns>
        private int GetCreatedPhaseDesignationValue(int origDesig, int destDesig)
        {
            if (destDesig <= 7 && destDesig >= 1)
            {
                return (origDesig | destDesig);
            }
            else
            {
                return origDesig;
            }
        }

        /// <summary>
        /// Returns new phasedesignation value.
        /// </summary>
        /// <param name="origDesig">the original phasedesignations value.</param>
        /// <param name="destDesig">the phasedesignation value being deleted/unrelated.</param>
        /// <returns>new phasedesignation value based on original and deleted/unrelated values.</returns>
        private int GetDeletedPhaseDesignationValue(int origDesig, int destDesig)
        {
            if ((origDesig & destDesig) != 0 && origDesig >= destDesig)
            {
                return (origDesig ^ destDesig);
            }
            else
            {
                return origDesig;
            }
        }

        #endregion helper methods

        /// <summary>
        /// Takes care of Number of phases if Relationship Created/Deleted on Conductor and ConductorInfo.
        /// </summary>
        /// <param name="featureClassRow">The OriginObject from Relationship.</param>
        /// <param name="destSet">All related objects set.</param>
        /// <param name="destRow">The DestObject from Relationship.</param>
        /// <param name="eEvent">The Event Created/Delted</param>
        private void ActualExecuteForSumUnitCount(IRow featureClassRow, ESRI.ArcGIS.esriSystem.ISet destSet, bool onlyValidate)
        {
            int CEDSANumberofPhases = 0;

            IObject featureObject = featureClassRow as IObject;
            IObjectClass featureObjectClass = featureObject.Class;

            //set the flag if featureclass is PrimaryOHconductor or PrimaryUGConductor
            bool isPrimary = IsPrimary(featureObjectClass);

            //get the field index from model name
            int updateCountIdx = ModelNameFacade.FieldIndexFromModelName(featureObjectClass, SchemaInfo.Electric.FieldModelNames.NumberOfPhases);

            //checkk if field index was not found
            if (updateCountIdx == -1)
            {
                _logger.Warn(SchemaInfo.Electric.FieldModelNames.NumberOfPhases + "-FieldModelname is not assigned for featureclass " + featureObjectClass.AliasName + ", exiting.");
                return;
            }

            int existingValue = 0;
            //get the phase count
            CEDSANumberofPhases = GetInfoPhaseCount(isPrimary, destSet);
            if (!int.TryParse(featureClassRow.get_Value(updateCountIdx).ToString(), out existingValue))
            {
                //parse failed means its null
                existingValue = 0;
            }

            //prepare the error message
            string validationError = string.Format(_cancelEditError, featureObjectClass.AliasName, featureClassRow.OID);

            //check if call is from validation rule
            if (onlyValidate)
            {
                //check for PhaseCount value and actual PhaseCount (from Info records)
                if (existingValue != CEDSANumberofPhases)
                {
                    //add the error if PhaseCount doesn't match
                    AddError(validationError);
                }
                return;
            }
            //check if conductorinfo phasecount is 0 means null then set the same in FC
            if (CEDSANumberofPhases == 0)
            {
                _logger.Debug("Value is <Null>, updating Number of phases.");
                featureClassRow.set_Value(updateCountIdx, DBNull.Value);

                //alert RelationshipAUs and SpecialAUs that an update on a parent is happening.
                try
                {
                    BaseRelationshipAU.IsRelAUCallingStore = true;
                    BaseSpecialAU.IsUnitCallingStore = true;
                    featureClassRow.Store();
                }
                finally
                {
                    BaseSpecialAU.IsUnitCallingStore = false;
                    BaseRelationshipAU.IsRelAUCallingStore = false;
                }

                _logger.Debug("Value <Null> updated in field Number of phases.");
            }
            //check for value against field domain
            else if (IsDomainValueValid(featureClassRow as IFeature, updateCountIdx, CEDSANumberofPhases))
            {
                _logger.Debug("Value is valid, updating Number of phases.");
                featureClassRow.set_Value(updateCountIdx, CEDSANumberofPhases);

                try
                {
                    //alert RelationshipAUs and SpecialAUs that an update on a parent is happening.
                    BaseRelationshipAU.IsRelAUCallingStore = true;
                    BaseSpecialAU.IsUnitCallingStore = true;
                    featureClassRow.Store();
                }
                finally
                {
                    BaseSpecialAU.IsUnitCallingStore = false;
                    BaseRelationshipAU.IsRelAUCallingStore = false;
                }

                _logger.Debug("Value updated in field Number of phases.");
            }
            else
            {
                //stop editing as this value is notvalid for domain.
                _logger.Error("Value is invalid, throwing MM_E_CANCELEDIT error.");
                throw new COMException(validationError, (int)mmErrorCodes.MM_E_CANCELEDIT);
            }
        }

        /// <summary>
        /// Get phase designation value calculated after considering Neutral.
        /// </summary>
        /// <param name="destSet">The set of Info records.</param>
        /// <param name="tempPhaseDesignation">The Phase Designation value.</param>
        /// <param name="phaseDesigIdx">The Phase Designation index.</param>
        /// <param name="useIdx">The Conductor use index.</param>
        /// <param name="isPrimary">The IsPrimary flag.</param>
        /// <returns></returns>
        private int GetPhaseDesignation(ESRI.ArcGIS.esriSystem.ISet destSet, int tempPhaseDesignation, int phaseDesigIdx, int useIdx, bool isPrimary)
        {
            IRow unitRow = null;
            //iterate for all set of info records
            while ((unitRow = destSet.Next() as IRow) != null)
            {
                //temporary ConductorUse value
                int useValue = 0;
                //temporary PhaseDesignation value
                int currentPhaseDesignation = 0;

                //parse values to integer
                int.TryParse(unitRow.get_Value(phaseDesigIdx).ToString(), out currentPhaseDesignation);

                //check if primary then consider checking ConductorUse
                if (isPrimary)
                {
                    int.TryParse(unitRow.get_Value(useIdx).ToString(), out useValue);
                    if (useValue != 1)
                    {
                        continue;
                    }
                }
                //check if Info record is neutral
                /////////////////////////new added start -24-oct 2012
                //RBAE - 9/10/2013 - check to see if phase designation value is neutral
                if (NeutralPhaseHelper.Instance.IsPhaseDesignationNeutral(unitRow as IObject))
                {
                    continue;
                }
                /////////////////////////new end start -24-oct 2012
                //generate the PhaseDesignation value
                tempPhaseDesignation = GetCreatedPhaseDesignationValue(tempPhaseDesignation, currentPhaseDesignation);
            }
            //return the calculated PhaseDesignation value
            return tempPhaseDesignation;
        }

        /// Updates PHASEDESIGNATION field in Feature class and take care if Object class is created/updated/deleted.
        /// </summary>
        /// <param name="featureClassRow">the feature </param>
        /// <param name="destSet">dataset which will have all related objects of the feature class</param>
        /// <param name="destRow">Object class row which is being updated</param>
        /// <param name="isPrimary">True if Model name is "PRIMARYCONDUCTOR"</param>
        private void ActualExecuteForPhaseDesignation(IRow featureClassRow, ESRI.ArcGIS.esriSystem.ISet destSet, IRow destRow)
        {
            destSet.Reset();
            int tempPhaseDesignation = 0;
            int tempPhaseDestDesignation = 0;
            //set the first object of ISet to unitRow
            IRow unitRow = destSet.Next() as IRow;
            //check if unitRow is null means no data retrieved
            if (unitRow == null)
            {
                _logger.Debug("ResultSet is <Null>, exiting.");
                //no realated conductor info is found means all records are removed.
                return;
            }

            //cast destRow to RowChanges so we can get original value
            IRowChanges destRowChanges = destRow as IRowChanges;
            IObject unitRowObject = unitRow as IObject;
            IObject featureObject = featureClassRow as IObject;

            IObjectClass unitFeatureClass = unitRowObject.Class;
            IObjectClass featureObjectClass = featureObject.Class;

            //get flag for Primary Conductor
            bool isPrimary = IsPrimary(featureObjectClass);

            //get the field indexes of Phase designation and ConductorUse
            int phaseDesigIdx = ModelNameFacade.FieldIndexFromModelName(unitFeatureClass, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
            int useIdx = ModelNameFacade.FieldIndexFromModelName(unitFeatureClass, SchemaInfo.Electric.FieldModelNames.CondutorUse);
            int updatePhaseDesigIdx = ModelNameFacade.FieldIndexFromModelName(featureObjectClass, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);

            //check if field model name is not found
            if (phaseDesigIdx == -1)
            {
                //log the warning and return the control
                _logger.Warn("phaseDesigIdx is -1, exiting.");
                return;
            }

            //check if primary then need to check ConductorUse
            if (isPrimary)
            {
                //check if field model name is not found
                if (useIdx == -1)
                {
                    _logger.Warn("useIdx is -1, exiting.");
                    //log the warning and return the control
                    return;
                }
            }

            //check if field model name is not found
            if (updatePhaseDesigIdx == -1)
            {
                //log the warning and return the control
                _logger.Warn("UpdatePhaseDesigIdx is -1, exiting.");
                return;
            }

            ////convert the value of PhaseDesignation to Integer value
            //if (!int.TryParse(featureClassRow.get_Value(updatePhaseDesigIdx).ToString(), out tempPhaseDesignation))
            //{
            //    //log the warning and return the control
            //    _logger.Warn("Orig phaseDesig conversion error.");
            //    //return;
            //}

            //convert the value of PhaseDesignation to Integer value
            if (!int.TryParse(destRowChanges.get_OriginalValue(phaseDesigIdx).ToString(), out tempPhaseDestDesignation))
            {
                //log the warning and return the control
                _logger.Warn("Dest phaseDesig conversion error.");
                //return;
            }

            //if (IsDeletionofPhaseCalculationNecessary(destSet, destRowChanges, phaseDesigIdx, useIdx))
            //{
            //    //remove the phase designation value from current phasedesignation even if event is delete or not
            //    _logger.Debug(string.Format("OrigPhaseDesignation:{0}, DestPhaseDesignation:{1}.", tempPhaseDesignation, tempPhaseDestDesignation));
            //    tempPhaseDesignation = GetDeletedPhaseDesignationValue(tempPhaseDesignation, tempPhaseDestDesignation);
            //    _logger.Debug(string.Format("tempPhaseDesignation = {0}", tempPhaseDesignation));
            //}

            //get the phase designation value for all related set
            destSet.Reset();
            tempPhaseDesignation = GetPhaseDesignation(destSet, tempPhaseDesignation, phaseDesigIdx, useIdx, isPrimary);
            _logger.Debug(string.Format("tempPhaseDesignation = {0}.", tempPhaseDesignation));

            //check if value is valid for domain for phase designation field
            if (tempPhaseDesignation == 0)
            {
                _logger.Debug("PhaseDesignation value is Null or Neutral Info records, so no need to update the Conductor, exiting.");
                return;
            }
            else if (IsDomainValueValid(featureClassRow as IFeature, updatePhaseDesigIdx, tempPhaseDesignation))
            {
                _logger.Debug("Value is valid, Setting value to PhaseDesignation field.");
                featureClassRow.set_Value(updatePhaseDesigIdx, tempPhaseDesignation);

                try
                {
                    BaseRelationshipAU.IsRelAUCallingStore = true;
                    BaseSpecialAU.IsUnitCallingStore = true;
                    featureClassRow.Store();
                }
                finally
                {
                    BaseSpecialAU.IsUnitCallingStore = false;
                    BaseRelationshipAU.IsRelAUCallingStore = false;
                }

                _logger.Debug("Value Updated to PhaseDesignation field.");
            }
            else
            {
                ///for invalid value we need to set Null for time being but as per client clarification on this issue we can take diffrent action as well.
                _logger.Error("Value is invalid, throwing MM_E_CANCELEDIT error.");
                throw new COMException(_cancelPhaseError, (int)mmErrorCodes.MM_E_CANCELEDIT);
            }
        }

        /// <summary>
        /// Tests if deleting the phase designation is necessary.
        /// </summary>
        /// <param name="destSet">The destination set.</param>
        /// <param name="destRowChanges">The altered row.</param>
        /// <param name="phaseIdx">The phase index</param>
        /// <param name="useIdx">The 'Use' index.</param>
        /// <returns>True if the phase designation should be deleted; otherwise false.</returns>
        private bool IsDeletionofPhaseCalculationNecessary(ESRI.ArcGIS.esriSystem.ISet destSet, IRowChanges destRowChanges, int phaseIdx, int useIdx)
        {
            IRow row = destRowChanges as IRow;
            //check if row found simply if row not found then its delete event so return true
            if (destSet.Find(row))
            {

                //RBAE - 9/10/2013 - check to see if phase designation value is neutral
                if (NeutralPhaseHelper.Instance.IsPhaseDesignationNeutralBeforeEdit(row as IObject))
                {
                    return false;
                }

                //if row exist then its either create or update
                //in case update need to check if phase changed
                if (destRowChanges.get_ValueChanged(phaseIdx))
                {
                    return true;
                }
                //in case update need to check if use changed
                if (useIdx != -1)
                {
                    if (destRowChanges.get_ValueChanged(useIdx))
                    {
                        return true;
                    }
                }

                //in any case the phase and use are unchanged so return false 
                //means no need to calculate phase based on deletion of this phase
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns ISet after removing deleted row from passed set if event is mmEventFeatureDelete.
        /// </summary>
        /// <param name="relatedSet">The Related set.</param>
        /// <param name="currentRow">The Current Object.</param>
        /// <param name="eEvent">The Event.</param>
        /// <returns></returns>
        private ESRI.ArcGIS.esriSystem.ISet GetValidSet(ESRI.ArcGIS.esriSystem.ISet relatedSet, IRow currentRow, mmEditEvent eEvent)
        {
            //check if event is featureDelete
            if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
            {
                //remove the current object if exist
                relatedSet.Remove(currentRow);
            }
            //return the related set
            return relatedSet;
        }
        #endregion private

        #region Public
        #region SumUnitCount
        /// <summary>
        /// Updates PhaseCount on FeatureClass to maintain relationship.
        /// </summary>
        /// <param name="relationship">The Relationship between FeatureClass and ObjectClass.</param>
        /// <param name="eEvent">The Event (Relationship Created/Deleted).</param>
        public void ExecuteForSumUnitRelation(IRelationship relationship, mmEditEvent eEvent)
        {
            //Parent object row
            IObject origObject = relationship.OriginObject;
            //Object tobe created or deleted
            IObject destObject = relationship.DestinationObject;

            //cast IObject to Irow
            IRow origRow = origObject as IRow;
            IRow destRow = destObject as IRow;

            //get the relationship class
            IRelationshipClass relClassConductor_ConductorInfo = relationship.RelationshipClass;
            //get all related objects to the object
            ESRI.ArcGIS.esriSystem.ISet relatedSet = relClassConductor_ConductorInfo.GetObjectsRelatedToObject(origObject);
            //remove the object if being deleted and found in set of records
            relatedSet = GetValidSet(relatedSet, destRow, eEvent);
            //process sumunit count
            ActualExecuteForSumUnitCount(origRow, relatedSet, false);
        }

        /// <summary>
        /// Updates PhaseCount on FeatureClass to maintain relationship.
        /// </summary>
        /// <param name="updatedObject">The Object to be Updated.</param>
        /// <param name="eEvent">The Event (Object Updated).</param>
        public void ExecuteForSumUnitInfoUpdate(IObject updatedObject, Miner.Interop.mmEditEvent eEvent)
        {
            //cast IObject to IRow
            IRow ObjectClassRow = updatedObject as IRow;
            //check if casting is succesful
            if (ObjectClassRow != null)
            {
                ITable ObjectClassTable = ObjectClassRow.Table;
                IRowChanges changeRow = updatedObject as IRowChanges;
                if (changeRow != null)
                {
                    //get field index from field model name
                    int useFieldNameIdx = ModelNameFacade.FieldIndexFromModelName(ObjectClassTable as IObjectClass, SchemaInfo.Electric.FieldModelNames.CondutorUse);
                    int countFieldNameIdx = ModelNameFacade.FieldIndexFromModelName(ObjectClassTable as IObjectClass, SchemaInfo.Electric.FieldModelNames.CondutorCount);

                    //check if field model name found
                    if (useFieldNameIdx == -1)
                    {
                        _logger.Warn(SchemaInfo.Electric.FieldModelNames.CondutorUse + "-FieldModelname is not assigned for objectclass, exiting.");
                        return;
                    }
                    //check if field model name found
                    if (countFieldNameIdx == -1)
                    {
                        _logger.Warn(SchemaInfo.Electric.FieldModelNames.CondutorCount + "-FieldModelname is not assigned for objectclass, exiting.");
                        return;
                    }
                    //check ConductorCount or ConductorUse field is edited then only go ahead else no need to run this logic 
                    if (!(changeRow.get_ValueChanged(useFieldNameIdx) || changeRow.get_ValueChanged(countFieldNameIdx)))
                    {
                        _logger.Debug("No change in ConductorUse or PhaseCount field, exiting.");
                        return;
                    }
                    //Get parent Feature
                    //Get all related objects of conductorinfo objclass
                    //Check if parent feature is having model name "PRIMARYCONDUCTOR"

                    //get the relationshipClass from Model name
                    IRelationshipClass relationClassConductorInfo = ModelNameFacade.RelationshipClassFromModelName(updatedObject.Class, esriRelRole.esriRelRoleAny, new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryConductor, SchemaInfo.Electric.ClassModelNames.SecondaryFeeder });
                    if (relationClassConductorInfo == null)
                    {
                        //no relationship class found
                        _logger.Info("No Relationshipclass found, exiting.");
                        return;
                    }

                    _logger.Debug("Getting all related objects.");
                    //get all the related objects
                    ESRI.ArcGIS.esriSystem.ISet relatedOrigFeatureSet = relationClassConductorInfo.GetObjectsRelatedToObject(updatedObject);
                    _logger.Debug("Got all related objects.");

                    //Ideally relatedOrigFeatureSet should have only 1 row as we are refering to Parent Feature from FeatureClass
                    if (relatedOrigFeatureSet.Count != 1)
                    {
                        //Something is wrong or no feature from featureclass is related to this object, that means no need to run through the logic
                        _logger.Debug("Related objects count is (0), exiting.");
                        return;
                    }

                    //get the first record from set of records
                    IObject featureClassObject = relatedOrigFeatureSet.Next() as IObject;
                    //check if set of records is empty
                    if (featureClassObject != null)
                    {
                        //cast Iobject to IRow
                        IRow featureClassRow = featureClassObject as IRow;
                        //check if casting was successful
                        if (featureClassRow != null)
                        {
                            //cast table to IfeatureClass
                            IFeatureClass origFeatureClass = featureClassRow.Table as IFeatureClass;
                            //get all related objects
                            ESRI.ArcGIS.esriSystem.ISet relatedChildSet = relationClassConductorInfo.GetObjectsRelatedToObject(featureClassObject);
                            _logger.Debug("Executing ActualExecuteOnUpdate.");
                            //process all the related records to sum up PhaseCount
                            ActualExecuteForSumUnitCount(featureClassRow, relatedChildSet, false);
                            _logger.Debug("Executed ActualExecuteOnUpdate.");
                        }
                        else
                        {
                            _logger.Debug("FeatureClassRow is <Null>, exiting.");
                        }
                    }
                    else
                    {
                        _logger.Debug("featureClassObject is <Null>, exiting.");
                    }
                }
                else
                {
                    _logger.Debug("changeRow is <Null>, exiting.");
                }
            }
            else
            {
                _logger.Debug("ObjectClassRow is <Null>, exiting.");
            }
        }

        /// <summary>
        /// Validates and Returns List of Errors if exist.
        /// </summary>
        /// <param name="rowToValidate">The FeatureClass to be validated.</param>
        /// <returns>Error list.</returns>
        public List<string> ExecuteForValidateSumUnitCount(IRow rowToValidate)
        {
            //clear all the error messages before starting the validation of records.
            _validationErr = null;
            //cast Irow to Iobject
            IObject objectToValidate = rowToValidate as IObject;
            IFeatureClass origFeatureClass = objectToValidate.Class as IFeatureClass;
            //get the relationship class from modelname
            IRelationshipClass relationClassConductorInfo = ModelNameFacade.RelationshipClassFromModelName(origFeatureClass, esriRelRole.esriRelRoleAny, new string[] { SchemaInfo.Electric.ClassModelNames.ConductorInfo });
            //check if valid relationship class object found
            if (relationClassConductorInfo == null)
            {
                //no relationship class found
                _logger.Warn("No relationship class found, exiting.");
                return _validationErr;
            }
            //get all the related objects
            ESRI.ArcGIS.esriSystem.ISet relatedSet = relationClassConductorInfo.GetObjectsRelatedToObject(objectToValidate);

            _logger.Debug("Executing ActualIsValid.");
            //process the Sum Unit count for retrieved set of records
            ActualExecuteForSumUnitCount(objectToValidate, relatedSet, true);
            _logger.Debug("Executed ActualIsValid.");
            return _validationErr;
        }
        #endregion SumUnitCount

        #region PhaseDesignation
        /// <summary>
        /// Updates PhaseDesignation value in FeatureClassto maintain relationship.
        /// </summary>
        /// <param name="relationship">The Relationship between FeatureClass and ObjectClass.</param>
        /// <param name="eEvent">The Event (Relationship Created/Deleted).</param>
        public void ExecuteForPhaseDesignationRelation(IRelationship relationship, mmEditEvent eEvent)
        {
            //Parent object row
            IObject origObject = relationship.OriginObject;

            //check for not null
            if (origObject == null)
            {
                //log the warning and return the control 
                _logger.Warn("OriginObject is <Null>, exiting.");
                return;
            }
            IObjectClass origObjectClass = origObject.Class;
            if (origObjectClass == null)
            {
                //log the warning and return the control  
                _logger.Warn("OriginObjectClass is <Null>, exiting.");
                return;
            }

            //check for not null
            //Object tobe created or deleted
            IObject destObject = relationship.DestinationObject;
            if (destObject == null)
            {
                //log the warning and return the control  
                _logger.Warn("DestinationObject is <Null>, exiting.");
                return;
            }
            IObjectClass destObjectClass = destObject.Class;
            if (destObjectClass == null)
            {
                //log the warning and return the control  
                _logger.Warn("DestinationObjectClass is <Null>, exiting.");
                return;
            }

            //check both the featureclass contains field model name
            if (!ModelNameFacade.ContainsFieldModelName(origObjectClass, new string[] { SchemaInfo.Electric.FieldModelNames.PhaseDesignation }))
            {
                //log the warning and return the control
                _logger.Warn("OriginalObjectClass does not contain FieldModelName :" + SchemaInfo.Electric.FieldModelNames.PhaseDesignation + " , exiting.");
                return;
            }

            if (!ModelNameFacade.ContainsFieldModelName(destObjectClass, new string[] { SchemaInfo.Electric.FieldModelNames.PhaseDesignation }))
            {
                //log the warning and return the control
                _logger.Warn("DestinationObjectClass does not contain FieldModelName :" + SchemaInfo.Electric.FieldModelNames.PhaseDesignation + " , exiting.");
                return;
            }

            //cast IObject to IRow
            IRow origRow = origObject as IRow;
            IRow destRow = destObject as IRow;
            //check if dest row is neutral then no need to check any thing just return the control
            //RBAE - 9/10/2013 - check to see if phase designation value is neutral
            if (NeutralPhaseHelper.Instance.IsPhaseDesignationNeutral(destRow as IObject))
            {
                _logger.Debug("Since ConductorInfo record is Neutral no need to check anything on PhaseDesigntation, Exiting.");
                return;
            }
            //get the relationship class frommodel name
            IRelationshipClass relClassConductor_ConductorInfo = relationship.RelationshipClass;
            //get all related objects
            ESRI.ArcGIS.esriSystem.ISet relatedSet = relClassConductor_ConductorInfo.GetObjectsRelatedToObject(origObject);
            //remove if current row is being deleted and retrieved in set of related records
            relatedSet = GetValidSet(relatedSet, destRow, eEvent);

            _logger.Debug("Executing ActualExecuteForPhaseDesignation.");
            //process the PhaseDesignation on all related records
            ActualExecuteForPhaseDesignation(origRow, relatedSet, destRow);
            _logger.Debug("Executed ActualExecuteForPhaseDesignation.");
        }

        /// <summary>
        /// Updates PhaseDesignation on FeatureClass to maintain relationship.
        /// </summary>
        /// <param name="updatedObject">The Object to be Updated.</param>
        /// <param name="eEvent">The Event (Object Updated).</param>
        public void ExecuteForPhaseDesignationUpdate(IObject updatedObject, Miner.Interop.mmEditEvent eEvent)
        {
            //cast IObject to IRow
            IRow ObjectClassRow = updatedObject as IRow;
            IObjectClass objectClass = updatedObject.Class;
            //check if IObjectClass is not null
            if (objectClass != null)
            {
                //cast table to dataaset
                IDataset ObjectClassDataSet = objectClass as IDataset;
                if (ObjectClassDataSet != null)
                {
                    //cast IRow to IRowChanges
                    IRowChanges changeRow = updatedObject as IRowChanges;
                    //check if Casting issuccessful
                    if (changeRow != null)
                    {
                        //get the field index from model name
                        int useIdx = ModelNameFacade.FieldIndexFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.CondutorUse);
                        int phaseDesigIdx = ModelNameFacade.FieldIndexFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.PhaseDesignation);
                        _logger.Debug(string.Format("ConductorUseIndex : {0}, and PhaseDesignationIndex: {1}.", useIdx, phaseDesigIdx));

                        //check PhaseDesignation or ConductorUse field is edited then only go ahead else no need to run this logic 
                        if (!(changeRow.get_ValueChanged(useIdx) || changeRow.get_ValueChanged(phaseDesigIdx)))
                        {
                            _logger.Debug("No changes done in PhaseDesignation or ConductorUse fields, exiting.");
                            return;
                        }

                        //Get parent Feature
                        //Get all related objects of conductorinfo objclass
                        //Check if parent feature is having model name "PRIMARYCONDUCTOR"
                        //get the relationchipclass from modelname
                        IRelationshipClass relationClassConductorInfo = ModelNameFacade.RelationshipClassFromModelName(objectClass, esriRelRole.esriRelRoleAny, new string[] { SchemaInfo.Electric.ClassModelNames.Conductor });//featureWkSpace.OpenRelationshipClass(relationshipClassName);
                        //check if relationship class is valid object
                        if (relationClassConductorInfo == null)
                        {
                            //No realationship class foundfor this objectclass and Conductor class modelname.
                            _logger.Warn("relationClassConductorInfo is <Null>, exiting.");
                            return;
                        }

                        //get all the related objects by modelname
                        ESRI.ArcGIS.esriSystem.ISet relatedOrigFeatureSet = relationClassConductorInfo.GetObjectsRelatedToObject(updatedObject);

                        //Ideally relatedOrigFeatureSet should have only 1 row as we are refering to Parent Feature from FeatureClass
                        if (relatedOrigFeatureSet.Count != 1)
                        {
                            //Something is wrong or no feature from featureclass is related to this object, that means no need to run through the logic
                            _logger.Warn("RelatedOrigFeatureSet is <Null>, exiting.");
                            return;
                        }
                        //cast first related object record to IObject
                        IObject featureClassObject = relatedOrigFeatureSet.Next() as IObject;

                        //check if casting is successful
                        if (featureClassObject != null)
                        {
                            //cast Iobject to IRow
                            IRow featureClassRow = featureClassObject as IRow;
                            //check if casting is successful
                            if (featureClassRow != null)
                            {
                                //cast IRow table to IFeatureClass
                                IFeatureClass origFeatureClass = featureClassRow.Table as IFeatureClass;
                                //get all related objects
                                ESRI.ArcGIS.esriSystem.ISet relatedChildSet = relationClassConductorInfo.GetObjectsRelatedToObject(featureClassObject);
                                //cast IRow to IRowChanges
                                IRowChanges ObjectClassRowChanges = ObjectClassRow as IRowChanges;
                                _logger.Debug("Executing ActualExecuteForPhaseDesignation.");
                                //process PhaseDesignation on all related records
                                ActualExecuteForPhaseDesignation(featureClassRow, relatedChildSet, ObjectClassRow);
                                _logger.Debug("Executed ActualExecuteForPhaseDesignation.");
                            }
                            else
                            {
                                _logger.Warn("FeatureClassRow is <Null>.");
                            }
                        }
                        else
                        {
                            _logger.Warn("FeatureClass is <Null>.");
                        }
                    }
                    else
                    {
                        _logger.Warn("ChangeRow is <Null>.");
                    }
                }
                else
                {
                    _logger.Warn("ObjectClassDS is <Null>.");
                }
            }
            else
            {
                _logger.Warn("ObjectClass is <Null>.");
            }
        }
        #endregion PhaseDesignation
        #endregion Public
    }
}
