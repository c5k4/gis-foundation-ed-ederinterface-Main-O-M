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
    [Guid("E13831CA-4C51-454F-B6E2-2842FA9994FF")]
    [ProgId("PGE.Desktop.EDER.ValidateRegionalAttributes")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateRegionalAttributes : BaseValidationRule
    {
        #region Constructors
        public ValidateRegionalAttributes()
            : base("PGE Validate Regional Attributes", _modelNames)
        {
        }
        #endregion Constructors

        #region private
        private const string _modelNames = SchemaInfo.Electric.ClassModelNames.ValidateRegionalAttributes;
        private const string _errMsg = "The feature ({0} OID: {1}) is not within proper organizational boundaries.";

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Returns the field model names for which this validation rule is defined for
        /// </summary>
        /// <returns></returns>
        private List<string> GetModelNamesList()
        {
            List<string> list = new List<string>();
            _logger.Debug("Creating list of field model names to validate.");
            //Assigning field model names to string array
            string[] tempList = new string[] { SchemaInfo.Electric.FieldModelNames.InheritCounty, SchemaInfo.Electric.FieldModelNames.InheritDistrict, SchemaInfo.Electric.FieldModelNames.InheritZip, SchemaInfo.Electric.FieldModelNames.InheritDivision, SchemaInfo.Electric.FieldModelNames.InheritRegion, SchemaInfo.Electric.FieldModelNames.InheritCity }; //, SchemaInfo.Electric.FieldModelNames.InheritLocalOffice 
            foreach (string str in tempList)
            {
                //add each field model name to List
                list.Add(str);
            }
            _logger.Debug("Created list of field model names to validate.");

            //return the prepared list of field model names
            return list;
        }

        #endregion private

        #region Overrides for validate regional attributes

        /// <summary>
        /// Validation rule to perform check as per defined rule.
        /// </summary>
        /// <param name="row">The Row being validated (QA/QC).</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //Change to address INC000003803926 QA/QC freezing on the QAQC subtask  
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            //Cast IRow to IObject and check for valid casting by checking null value
            IObject rowObject = row as IObject;
            if (rowObject != null)
            {
                IObjectClass rowObjectClass = rowObject.Class;
                string featureClassName = rowObjectClass.AliasName;
                int OID = row.HasOID ? row.OID : -1;
                //prepare error message
                string errMsg = string.Format(_errMsg, featureClassName, OID == -1 ? "" : OID + "");
                _logger.Debug("Executing GetModelNamesList.");
                //get the list of field model names for which validation is to be done
                List<string> listOfFieldModelNames = GetModelNamesList();
                _logger.Debug("Executed GetModelNamesList.");
                //check if list of field model names is not null
                if (listOfFieldModelNames != null)
                {
                    _logger.Debug("Loop through all Field model names to validate, if exist.");
                    //check for each field model names from list
                    foreach (string fieldModelName in listOfFieldModelNames)
                    {
                        //get the field index if present in field  
                        int fieldIdx = ModelNameFacade.FieldIndexFromModelName(rowObjectClass, fieldModelName);
                        if (fieldIdx == -1)
                        {
                            _logger.Debug(fieldModelName + " field model names doesnt exist on " + rowObjectClass.AliasName + ".");
                            continue;
                        }
                        _logger.Debug(fieldModelName + " field model names exist on " + rowObjectClass.AliasName + " field index:" + fieldIdx + ", validating.");

                        //get the field value and check for null
                        string fieldValue = rowObject.GetFieldValue(null, false, fieldModelName).Convert("<Null>");
                        if (fieldValue.Equals("<Null>"))
                        {
                            //if field value is null then just add the prepared error
                            _logger.Debug(errMsg);
                            AddError(errMsg);
                            break;
                        }
                    }
                }
                _logger.Debug("Loop end."); 
            }
            else
            {
                _logger.Warn("List of model names is <Null>, exiting.");
            }
            return _ErrorList;
        }
        #endregion Overrides for validate regional attributes
    }
}
