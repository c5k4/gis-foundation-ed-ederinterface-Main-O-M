using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("C6BF744A-0C82-46D4-BA5E-2F8FA2DDD917")]
    [ProgId("PGE.Desktop.EDER.ValidateProposedChange")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateProposedChange : BaseValidationRule
    {

        #region Constructor
        public ValidateProposedChange()
            : base("PGE Validate Proposed Change", _modelNames)
        {
        }
        #endregion Constructor

        #region Private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string[] _modelNames = new String[] { SchemaInfo.Electric.ClassModelNames.PropsedChange };
        private const int _statusProposedChange = 1;
        private const int _statusProposedInstall = 0;
        private const string _err_In_Status_Change = "OID: {0} has a ParentID {1}. GUID: {1} this feature status should be set to PROPOSED CHANGE.";
        //private const string _err_In_Status_Change = "OID: {0} has a ParentID {1} and this feature status should be set to PROPOSED CHANGE";
        private const string _err_In_Status_Install = "OID: {0} status is set to Proposed Change. A Replacing object is not found. Set the ParentID of the objectID replacing this object to {1}.";
        private const string _err_Multiple_records = "Multiple {0} features are marked to replace one feature. Check ParentID of the features OID: {1}.";
        private const string _err_Parent_ID_Null = "Parent ID is <Null>.";
        private string statusFieldName = "";

        /// <summary>
        /// Validates if SAPParentID field is not null.
        /// </summary>
        /// <param name="objToCheck">The Object to check.</param>
        /// <returns></returns>
        private bool IsSAPParentIDOK(IObject objToCheck)
        {
            //get the value for parentID field
            string objectValue = objToCheck.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.SAPParentID).Convert("<null>");
            if (objectValue.Equals("<null>"))
            {
                //return false as it contains null
                _logger.Debug("Feature Class:" + objToCheck.Class.AliasName + " field with model name: " + SchemaInfo.Electric.FieldModelNames.SAPParentID + ", value is <Null> or featuredoesnt have field model name present.");
                return false;
            }
            else
            {
                //return true since everything is ok
                _logger.Debug("Value is ok.");
                return true;
            }

            //int idx = ModelNameFacade.FieldIndexFromModelName(objToCheck.Class, SchemaInfo.Electric.FieldModelNames.SAPParentID);
            //if (idx != -1)
            //{
            //    //Check if equals to DBNull
            //    if (!DBNull.Value.Equals(objToCheck.get_Value(idx)))
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //    _logger.Warn("Feature Class:" + objToCheck.Class.AliasName + " doesn't have field with model name: " + SchemaInfo.Electric.FieldModelNames.SAPParentID);
            //}
            //return false;
        }

        /// <summary>
        /// Checks if object status is Proposed Change.
        /// </summary>
        /// <param name="objToCheck">The Object to check.</param>
        /// <returns></returns>
        private bool IsProposedChange(IObject objToCheck)
        {
            //get status field value and -1 if null or field model name doesnt exist
            int statusValue = objToCheck.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).Convert(-1);
            //check for valid value
            if (statusValue != -1)
            {
                statusFieldName = ModelNameFacade.FieldNameFromModelName(objToCheck.Class, SchemaInfo.Electric.FieldModelNames.Status);
                //check for ProposedChange constant value
                if (statusValue == _statusProposedChange)
                {
                    _logger.Debug("Status is " + statusValue + " and = " + _statusProposedChange);
                    return true;
                }
                else
                {
                    //log the details
                    _logger.Debug("Status is " + statusValue + " and not " + _statusProposedChange);
                }
            }
            else
            {
                //log the details
                _logger.Debug("Status is <Null>.");

            }
            return false;
            //int idx = ModelNameFacade.FieldIndexFromModelName(objToCheck.Class, SchemaInfo.Electric.FieldModelNames.Status);
            //if (idx != -1)
            //{
            //    statusFieldName = objToCheck.Fields.get_Field(idx).Name;
            //    object objVal = objToCheck.get_Value(idx);
            //    if (!DBNull.Value.Equals(objVal))
            //    {
            //        int value=0;
            //        if (int.TryParse(objVal.ToString(), out value))
            //        {
            //            //Check if value matches with ProposedChange constant
            //            if (value == _statusProposedChange)
            //            {
            //                return true;
            //            }
            //        }
            //        else
            //        {
            //            _logger.Debug("while parsing Status value to int errro occured, status value : " + objVal.ToString());
            //        }
            //    }
            //}
            //else
            //{
            //    _logger.Warn("Feature Class:" + objToCheck.Class.AliasName + " doesn't have field with model name: " + SchemaInfo.Electric.FieldModelNames.Status);
            //}
            //return false;
        }

        /// <summary>
        /// Checks if object status is Proposed Install.
        /// </summary>
        /// <param name="objToCheck">The Object to check.</param>
        /// <returns></returns>
        private bool IsProposedInstall(IObject objToCheck)
        {
            //get status field value and -1 if null or field model name doesnt exist
            int statusValue = objToCheck.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).Convert(-1);
            //check for valid value
            if (statusValue != -1)
            {
                statusFieldName = ModelNameFacade.FieldNameFromModelName(objToCheck.Class, SchemaInfo.Electric.FieldModelNames.Status);
                //check for proposedInstallconstant value
                if (statusValue == _statusProposedInstall)
                {
                    _logger.Debug("Status is " + statusValue + " and = " + _statusProposedInstall);
                    return true;
                }
                else
                {
                    //log the details
                    _logger.Debug("Status is " + statusValue + " and not " + _statusProposedInstall);
                }
            }
            else
            {
                //log the details
                _logger.Debug("Status is <Null>.");
            }
            return false;
            //int idx = ModelNameFacade.FieldIndexFromModelName(objToCheck.Class, SchemaInfo.Electric.FieldModelNames.Status);
            //if (idx != -1)
            //{
            //    statusFieldName = objToCheck.Fields.get_Field(idx).Name;
            //    object objVal = objToCheck.get_Value(idx);
            //    if (!DBNull.Value.Equals(objVal))
            //    {
            //        int value = 0;
            //        if (int.TryParse(objVal.ToString(), out value))
            //        {
            //            //Check if value matches with ProposedInstall constant
            //            if (value == _statusProposedInstall)
            //            {
            //                return true;
            //            }
            //        }
            //        else
            //        {
            //            _logger.Debug("while parsing Status value to int errro occured, status value : " + objVal.ToString());
            //        }
            //    }
            //}
            //else
            //{
            //    _logger.Warn("Feature Class:" + objToCheck.Class.AliasName + " doesn't have field with model name: " + SchemaInfo.Electric.FieldModelNames.Status);
            //}
            //return false;
        }

        /// <summary>
        /// Check for all the possible errors to be reported in QA/Qc.
        /// </summary>
        /// <param name="objToCheck">The Object to Check.</param>The 
        /// <param name="isProposedChange">Boolean flag to determine ProposedChange or ProposedInstall depending on Staus.</param>
        private void CheckForErrors(IObject objToCheck, bool isProposedChange)
        {
            //IQueryFilter to query featureclass and retrieve result
            IQueryFilter qf = new QueryFilter();
            IFeatureClass featureClassToCheck = objToCheck.Class as IFeatureClass;
            string parentIDFieldName = "";
            string parentIDFieldValue = "";
            string guIDFieldName = "";
            string guIDFieldValue = "";

            //Need ParentID field value and name so FieldIndex is used otherwise may have used Extensions method GetFieldValue.
            int idxParentId =ModelNameFacade.FieldIndexFromModelName(featureClassToCheck, SchemaInfo.Electric.FieldModelNames.SAPParentID);
            if (idxParentId != -1)
            {
                //since ParentId field present get the field name and field value.
                parentIDFieldName = objToCheck.Fields.get_Field(idxParentId).Name;
                parentIDFieldValue = DBNull.Value.Equals(objToCheck.get_Value(idxParentId)) ? "" : objToCheck.get_Value(idxParentId).ToString();
            }
            else 
            {
                //log the details regarding ParentId field doesnt exist
                _logger.Warn("FeatureClass : " + featureClassToCheck.AliasName + " doesn't have field model name assigned : " + SchemaInfo.Electric.FieldModelNames.SAPParentID);
                return;
            }

            //Get the GUID value and Field name if present
            if(!GetGUID(objToCheck,out guIDFieldName, out guIDFieldValue))
            {
                //log the details regarding GUID doesn't exist
                _logger.Warn("FeatureClass : " + featureClassToCheck.AliasName + " doesn't have GUID field.");
                return;
            }

            if (string.IsNullOrEmpty(statusFieldName))
            {
                //log the details regarding GUID doesn't exist
                _logger.Warn("FeatureClass : " + featureClassToCheck.AliasName + " doesn't have STATUS field.");
                return;                
            }
            //Set Whereclause if status is ProposedChange
            if (isProposedChange)
            {
                //set whereclause as [PARENTID]=<GUIDVALUE> AND [STATUS]=<ProposedInstall>
                qf.WhereClause = string.Format("{0}='{1}' AND {2}={3}", parentIDFieldName, guIDFieldValue, statusFieldName, _statusProposedInstall);
            }
            // Set Whereclause if status is ProposedInstall
            else
            {
                //set whereclause as [GUID]=<ParentIDValue> AND [STATUS]=<ProposedChange>
                qf.WhereClause = string.Format("{0}='{1}' AND {2}={3}", guIDFieldName, parentIDFieldValue, statusFieldName, _statusProposedChange);
            }
            //Get the FeatureCount before geting cursor
            int featureCount = featureClassToCheck.FeatureCount(qf);
            if (featureCount != 1)
            {
                //check if count>1 then generate the Error message for Multiple features
                if (featureCount > 1)
                {
                    //since featureCount is !=1 we need to retrieve all the records in featureCursor
                    IFeatureCursor featureCursor = featureClassToCheck.Search(qf, false);
                    try
                    {
                        //generate the list of OID for each feature
                        string oidList = GetOIDList(featureCursor);
                        //prepare the error and add the same to log and Error List
                        string err = string.Format(_err_Multiple_records, featureClassToCheck.AliasName, oidList);
                        _logger.Debug(err);
                        AddError(err);
                    }
                    finally
                    {
                        //Release the COMObject Ifeaturecursor
                        while (Marshal.ReleaseComObject(featureCursor) > 0)
                        { }
                    }

                }
                else//No record found
                {
                    if (!isProposedChange)
                    {
                        //log the details and add the error
                        string err = string.Format(_err_In_Status_Change, objToCheck.HasOID ? objToCheck.OID + "" : "", parentIDFieldValue);
                        _logger.Debug(err);
                        AddError(err);
                    }
                    else
                    {
                        //log the details and add the error
                        string err = string.Format(_err_In_Status_Install, objToCheck.HasOID ? objToCheck.OID + "" : "", guIDFieldValue);
                        _logger.Debug(err);
                        AddError(err);
                    }
                }
            }
            else
            {
                _logger.Debug("No errorin validating ProposedChange.");
            }
        }

        /// <summary>
        /// Generates list of OID saperated by ","
        /// </summary>
        /// <param name="featureCursor">The Feature Cursor.</param>
        /// <returns></returns>
        private string GetOIDList(IFeatureCursor featureCursor)
        {
            //prepare the list of OID
            List<string> listOfOID = new List<string>();
            IFeature tmpFeature = null;
            //run through featureCursor
            while ((tmpFeature = featureCursor.NextFeature()) != null)
            {
                //check if OID field is present
                if (tmpFeature.HasOID)
                {
                    //add the OID to list
                    listOfOID.Add(tmpFeature.OID.ToString());
                }
            }
            //concatenate all the OID in string and return
            return string.Join(",", listOfOID.ToArray());
        }

        /// <summary>
        /// Check if GUID field is present and then return GUID field value and Field name.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="GUIDFieldName">Out param GUIDFieldName.</param>
        /// <param name="GUIDValue">Out param GUIDValue.</param>
        /// <returns>Returns true if GUID field found.</returns>
        private bool GetGUID(IObject feature, out string GUIDFieldName, out string GUIDValue)
        {
            //cast IobjectClass to featureclass
            IFeatureClass featureClass = feature.Class as IFeatureClass;
            //cast IFeatureClass to IclassEx
            IClassEx featureClassEx = featureClass as IClassEx;
            //check if GUID field is present
            if (featureClassEx.HasGlobalID)
            {
                //get the field name 
                GUIDFieldName = featureClassEx.GlobalIDFieldName;
                //get the field value empty string if null
                GUIDValue = feature.GetFieldValue(GUIDFieldName, false, null).Convert("");
                return true;

                //object objValue = feature.get_Value(feature.Fields.FindField(GUIDFieldName));
                //if (DBNull.Value.Equals(objValue))
                //{
                //    GUIDValue = "";
                //}
                //else
                //{
                //    GUIDValue = objValue.ToString();
                //}
            }
            else
            {
                //since no GUID field is available clear the OUT params and return false
                GUIDFieldName = "";
                GUIDValue = "";
                return false;
            }
        }

        /// <summary>
        /// Check for all defined errors.
        /// </summary>
        /// <param name="objToCheck">The Object to Check.</param>
        private void ActualIsValid(IObject objToCheck)
        {
            //Get value for Is proposed and Is Install based on Status value
            bool isProposedChange = IsProposedChange(objToCheck);
            bool isProposedInstall = IsProposedInstall(objToCheck);
            //Check if atleast Status is either ProposedChange or ProposedInstall
            if (isProposedChange || isProposedInstall)
            {
                //check if Proposd Install
                if (isProposedInstall)
                {
                    //validate ParentID for staus =Proposed Install 
                    if (!IsSAPParentIDOK(objToCheck))
                    {
                        //log the details as Parent ID is not ok
                        _logger.Debug("Parent id is <Null>.");
                        return;
                    }
                }
                //check for common validations for ProposedChange or ProposedInstall
                CheckForErrors(objToCheck, isProposedChange);
            }
            else
            {
                //no need to validate the object since Status isnot ProposedChange nor ProposedInstall
                _logger.Debug("Status is not Proposed Change nor Proposed Install.");
            }
        }
        #endregion Private

        #region Override Validation rule
        /// <summary>
        /// Validates selected Object against defined rules for ProposedChange.
        /// </summary>
        /// <param name="row">The Selected Row/Object.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //Change to address INC000003803926 QA/QC freezing on the QAQC subtask 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            //cast selectedRow to Iobject
            IObject unitObject = row as IObject;
            //check if casting was successful
            if (unitObject != null)
            {
                //check for defined errors
                ActualIsValid(unitObject);
            }
            else
            {
                //log the details as casting was unsuccessful
                _logger.Debug("Object is <Null>, not able to validate.");
            }
            //return error list if any
            return _ErrorList;
        }
        #endregion Override Validation rule
    }
}

