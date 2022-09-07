using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    /// <summary>
    /// Serializes and deserialized a FEATURECLASS element in the symbol number config.
    /// </summary>
    public class FeatureClass
    {
        #region Properties
        /// <summary>
        /// Gets or sets the criteria.  A top level object which contains rules.
        /// </summary>
        /// <value>
        /// The criteria.
        /// </value>
        [XmlElement(typeof(Rule), ElementName = "Rule")]
        public List<Rule> Rules { get; set; }

        /// <summary>
        /// Gets or sets the feature class name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        //[XmlAttribute]
        //public string Name { get; set; }

        /// <summary>
        /// Gets or sets the default symbol.  If no symbol value is found use this.
        /// </summary>
        /// <value>
        /// The default symbol.
        /// </value>
        [XmlElement(typeof(int))]
        public int DefaultSymbol { get; set; }
        #endregion Properties

        #region Constructor
        public FeatureClass()
        {
            DefaultSymbol = -1;
        }
        #endregion Constructor

        #region SymbolNumber
        #region Public

        /// <summary>
        /// Gets the symbol number by evaluating symbol number criteria.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public int GetSymbolNumber(IFeature feature)
        {
            _logger.Debug("Executing evaluating each rule to get valid symbol number.");
            //iterate through every rule
            foreach (Rule rules in Rules)
            {
                // Check for a criteria match.
                if (rules.Evaluate(feature))
                {
                    // Criteria evaluated to true, return the symbol number defined in the root criteria.
                    _logger.Debug(string.Format("Rule with symbol number {0} evaluated to true.", rules.SymbolNumber));
                    return rules.SymbolNumber;
                }
            }
            // No match return the default value.
            _logger.Debug(string.Format("No rule with is evaluated to true. returning default {0} symbol number.", DefaultSymbol));
            return DefaultSymbol;
        }

        /// <summary>
        /// Initializes the specified object class, and all subclasses.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(IObjectClass objectClass)
        {
            //iterate for each rule to initialize
            foreach (Rule criterion in Rules)
            {
                //initialize the rule
                _logger.Debug(string.Format("Initializing the rule with symbol number {0} for {1} object class.", criterion.SymbolNumber, objectClass.AliasName));
                criterion.Initialize(objectClass);
                _logger.Debug(string.Format("Successfully initialized the rule with symbol number {0} for {1} object class.", criterion.SymbolNumber, objectClass.AliasName));
            }
        }

        /// <summary>
        /// Appends all model names from child elements to a list.
        /// </summary>
        /// <param name="modelNames">The model names.</param>
        public void AppendModelNames(List<string> modelNames)
        {
            //iterate for all the rules to append the model names
            foreach (Rule criterion in Rules)
            {
                _logger.Debug(string.Format("Appending the model names for rule with symbol number {0}", criterion.SymbolNumber));
                criterion.AppendModelNames(modelNames);
                _logger.Debug(string.Format("Successfully executed the append model names for rule with symbol number {0}", criterion.SymbolNumber));
            }
        }
        #endregion Public
        #endregion SymbolNumber

        #region OperatingNumber
        #region Public
        /// <summary>
        /// Gets the operating number errors by evaluating operating number criteria.
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <returns></returns>
        public List<string> ValidateOperatingNumber(IFeature feature)
        {
            //iterate for all the rules
            foreach (Rule rules in Rules)
            {
                // Check for a criteria match.
                if (rules.Evaluate(feature))
                {
                    // Criteria evaluated to true, return the symbol number defined in the root criteria.
                    HandleRulesFromRulesTable(feature, rules);
                    return _err_List;
                }
            }
            // No match return the default value.
            return null;
        }
        #endregion Public

        #region Private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");        
        //*****************Incident Number INC000003777409 **********************
        private const string _err_Duplicate_Operating_Number = "{0} OID {1} has duplicate Operating Number on Circuit {3} in {2} division.";
        //***********************************************************************
        private const string _err_Operating_Number_Length_GT_Max = "Operating Number Format Error: Number should be at most {0} characters.";
        private const string _err_Operating_Number_Starts_With = "Operating Number Format Error: Invalid Identifier, {0} operating number should start with {1}.";
        private const string _err_Operating_Number_End_Even_or_Odd = "Operating Number Format Error: Number should be {0}.";
        private const string _err_Operating_Number_is_required = "Operating Number is required.";
        private const string _err_Operating_Numer_Too_Short = "Operating Number Format Error: Number should be at least {0} characters.";
        private const string _err_Operating_Number_Spcl_Char = "Operating Number Format Error: \"{0}\" is not a valid character.";
        private const string _err_Operating_Number_Padded_Zero = "Operating Number Format Error: Number contains extra zeros at the beginning.";

        private List<string> _err_List = null;

        /// <summary>
        /// Returns True if Required and value is null.
        /// </summary>
        /// <param name="operatingNumber">The OperatingNumber.</param>
        /// <param name="isRequired">True if Required.</param>
        /// <returns></returns>
        private bool CheckRequiredAndNull(object operatingNumber, bool isRequired)
        {
            //check if rule value for required is true
            if (isRequired)
            {
                //check value is null
                if (operatingNumber == null || operatingNumber.Equals(DBNull.Value))
                {
                    //add the defined error
                    _logger.Debug(string.Format(_err_Operating_Number_is_required));
                    AddError(string.Format(_err_Operating_Number_is_required));
                    //return true as operating number isrequired and still null
                    return true;
                }
            }
            //since it optional return false.
            return false;
        }

        /// <summary>
        /// Checks if OperatingNumber starts with valid character and no padded 0's.
        /// </summary>
        /// <param name="operatingNumber">The OperatingNumber.</param>
        /// <param name="beginsWith">The Valid character to begin with.</param>
        /// <param name="aliasName">The AliasName of to beadded in error.</param>
        private void CheckBeginsWithAndPaddedZero(string operatingNumber, string beginsWith, string aliasName)
        {
            //Check if Operating number is starts with valid defined char
            if (!operatingNumber.StartsWith(beginsWith))
            {
                string errStr = string.Format(_err_Operating_Number_Starts_With, aliasName, beginsWith);
                _logger.Debug(errStr);
                AddError(errStr);
            }

            //Current case Begins with and padded zero takes care even if valid begins with but next char is zero then this will report error
            //incase if this need to change we can simply replace tempOpNum with operatingNumber variable.

            //Check if Operating number do not have padded zero
            string tempOpNum = new string(operatingNumber.ToCharArray(beginsWith.Length, operatingNumber.Length - beginsWith.Length));
            if (tempOpNum.StartsWith("0"))
            {
                string errStr = string.Format(_err_Operating_Number_Padded_Zero);
                _logger.Debug(errStr);
                AddError(errStr);
            }
        }

        /// <summary>
        /// Checks if OperatingNumber ends with Odd/Even number
        /// </summary>
        /// <param name="operatingNumber">The OperatingNumber.</param>
        /// <param name="endsWith">The EndsWith criteria.</param>
        private void CheckEndsWith(string operatingNumber, int endsWith)
        {

            _logger.Debug("Ends with value is :" + endsWith);
            bool isEndsWithEven = endsWith == 0 ? true : false;
            bool isEndsWithOdd = endsWith == 1 ? true : false;
            int endsWithNum = 0;
            //check if last digit is integer
            if (int.TryParse(operatingNumber[operatingNumber.Length - 1] + "", out endsWithNum))
            {
                //check if according to rule operating number should end with even number
                if (isEndsWithEven)
                {
                    //validate for even number ends with
                    if (endsWithNum % 2 != 0)
                    {
                        //log the error as number is not ends with even number
                        string errStr = string.Format(_err_Operating_Number_End_Even_or_Odd, "Even");
                        _logger.Debug(errStr);
                        AddError(errStr);
                    }
                }
                //check if according to rule operating number should end with odd number
                if (isEndsWithOdd)
                {
                    //validate for odd number ends with
                    if (endsWithNum % 2 != 1)
                    {
                        //log the error as number is not ends with odd number
                        string errStr = string.Format(_err_Operating_Number_End_Even_or_Odd, "Odd");
                        _logger.Debug(errStr);
                        AddError(errStr);
                    }
                }
            }
            else
            {
                //log the warning of operating number is not ending with number
                _logger.Debug("Operatingnumber doesn't endwith number.");
                if (isEndsWithEven)
                {
                    //log the error as number is not ends with even number
                    string errStr = string.Format(_err_Operating_Number_End_Even_or_Odd, "Even");
                    _logger.Debug(errStr);
                    AddError(errStr);
                }
                if (isEndsWithOdd)
                {
                    //log the error as number is not ends with odd number
                    string errStr = string.Format(_err_Operating_Number_End_Even_or_Odd, "Odd");
                    _logger.Debug(errStr);
                    AddError(errStr);
                }
            }
        }

        /// <summary>
        /// Checks OperatingNumber for minimum required length.
        /// </summary>
        /// <param name="operatingNumber">The OperatingNumber</param>
        /// <param name="minLength">The MinimumLength.</param>
        private void CheckMinLenght(string operatingNumber, int minLength)
        {
            //check for valid minimum required length of operating number
            if (operatingNumber.Length < minLength)
            {
                string errStr = string.Format(_err_Operating_Numer_Too_Short, minLength);
                _logger.Debug(errStr);
                AddError(errStr);
            }
        }

        /// <summary>
        /// Checks OperatingNumber for maximum length.
        /// </summary>
        /// <param name="operatingNumber">The OperatingNumber.</param>
        /// <param name="maxLength">The Maximum valid length.</param>
        private void CheckMaxLength(string operatingNumber, int maxLength)
        {
            //check for valid maximum length of operatingnumber
            if (operatingNumber.Length > maxLength)
            {
                string errStr = string.Format(_err_Operating_Number_Length_GT_Max, maxLength);
                _logger.Debug(errStr);
                AddError(errStr);
            }
        }

        /// <summary>
        /// Checks OperatingNumber for valid characters.
        /// </summary>
        /// <param name="operatingNumber">The OperatingNumber.</param>
        /// <param name="beginsWith">The valid Character to begins with.</param>
        private void CheckOperatingNumberValidChars(string operatingNumber, string beginsWith)
        {
            //check the operating number for valid characters 
            //valid characters are BeginsWith and remaining all digits should be number.
            int ctr = 0;
            for (ctr = beginsWith.Length; ctr < operatingNumber.Length; ctr++)
            {
                if (isCharValid(operatingNumber[ctr]))
                {
                    continue;
                }
                string errStr = string.Format(_err_Operating_Number_Spcl_Char, operatingNumber[ctr]);
                _logger.Debug(errStr);
                AddError(errStr);
            }
        }

        private bool isCharValid(char ch)
        {
            if ((ch <= '9' && ch >= '0') || (ch<='z' && ch>='a') || (ch<='Z' && ch>='A'))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Checks OperatingNumber for duplicate all across the defined ModelName.
        /// </summary>
        /// <param name="currentFeature">The CurrentFeature.</param>
        /// <param name="operatingNumberIdx">TheOperatingNumber field index.</param>
        private void CheckDuplicateFeatures(IFeature currentFeature, int operatingNumberIdx)
        {
            //check for the duplicate operating number in all across workspace with defined model name
            IDataset currentFeatureDS = currentFeature.Class as IDataset;
            if (currentFeatureDS == null)
            {
                return;
            }
            _logger.Debug("Executing HandleDuplicateErr.");
            HandleDuplicateErr(currentFeatureDS.Workspace, currentFeature, operatingNumberIdx);
            _logger.Debug("Executed HandleDuplicateErr.");
        }

        /// <summary>
        /// Returns All features from passed Featureclass where PGE_OPERATINGNUMBER field matche with selected feature 
        /// </summary>
        /// <param name="objectFeature">Feature selected for QA/QC.</param>
        /// <param name="objecFieldIdx">PGE_OPERATINGNUMBER field Index</param>
        /// <param name="currentFc">Featureclass from which features will be retrieved using query filter.</param>
        /// <param name="featureClassFieldName">PGE_OPERATINGNUMBER filed name.</param>
        /// <returns>Returns FeatureCursor from passed Featureclass where selected feature PGE_OPERATINGNUMBER value matches.</returns>
        private IFeatureCursor GetIFeatureCursor(IFeature objectFeature, int objecFieldIdx, IFeatureClass currentFc, string featureClassFieldName, out int divisionIdx, out int circuidIDIdx)
        {
            circuidIDIdx = -1;
            divisionIdx = -1;
            divisionIdx = ModelNameFacade.FieldIndexFromModelName(currentFc, SchemaInfo.Electric.FieldModelNames.Division);
            if (divisionIdx == -1)
            {
                //log the warning and continue the saerchfor next featureclass
                _logger.Warn(currentFc.AliasName + ": doesnt contain FieldModelName:" + SchemaInfo.Electric.FieldModelNames.Division + ", returning.");
                return null;
            }

            // PGE_CIRCUITID is the fieldmodelname for CircuitID
            circuidIDIdx = ModelNameFacade.FieldIndexFromModelName(currentFc, SchemaInfo.Electric.FieldModelNames.FeederID);
            if (circuidIDIdx == -1)
            {
                //log the warning and continue the saerchfor next featureclass
                _logger.Warn(currentFc.AliasName + ": doesnt contain FieldModelName:" + SchemaInfo.Electric.FieldModelNames.FeederID + ", exiting.");
                return null;
            }

            string divisionFieldName = currentFc.Fields.get_Field(divisionIdx).Name;
            string circuitIDFieldName = currentFc.Fields.get_Field(circuidIDIdx).Name;
            string OIDFieldName = currentFc.HasOID ? currentFc.OIDFieldName : null;
            if (OIDFieldName == null)
            {
                _logger.Warn(currentFc.AliasName + ": doesnt contain OID, exiting.");
                return null;
            }

            object featureFieldValue = objectFeature.get_Value(objecFieldIdx);
            //if same feature class then take care with <>OID so same feature will not be reported as duplicate 
            bool sameFeatureClass = objectFeature.Class.ObjectClassID == currentFc.ObjectClassID;
            IQueryFilter qfilter = new QueryFilter();
            ITopologicalOperator iTopo = (ITopologicalOperator)objectFeature.Shape;
            qfilter.WhereClause = "";
            if (sameFeatureClass)
            {
                if (objectFeature.HasOID)
                {
                    qfilter.WhereClause = currentFc.OIDFieldName + " <> " + (objectFeature.OID) + " and ";
                }
            }
            qfilter.WhereClause += featureClassFieldName + "='" + featureFieldValue + "'";
            //get only necessary fields to boost the performance
            qfilter.SubFields = OIDFieldName + ", " + circuitIDFieldName + ", " + divisionFieldName;
            _logger.Debug("QueryFilter whereClause:" + qfilter.WhereClause);
            int count = currentFc.FeatureCount(qfilter);
            _logger.Debug("Total features:" + count);
            IFeatureCursor SelectedCursor = currentFc.Search(qfilter, true);
            return SelectedCursor;
        }

        /// <summary>
        /// Handles Duplicate in OperatingNumber field all across defined ModelName.
        /// </summary>
        /// <param name="WkSpace">The Workspace.</param>
        /// <param name="currentFeature">The CurrentFeature.</param>
        /// <param name="operatingNumberIdx">The OperatingNumber field index.</param>
        private void HandleDuplicateErr(IWorkspace WkSpace, IFeature currentFeature, int operatingNumberIdx)
        {
            //check for duplicate features start
            IEnumFeatureClass enumFeatureClasses = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(WkSpace, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
            IFeatureClass tempFeatureClass = null;
            while ((tempFeatureClass = enumFeatureClasses.Next()) != null)
            {
                int tempOperatingNumberIdx = ModelNameFacade.FieldIndexFromModelName(tempFeatureClass, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                if (tempOperatingNumberIdx == -1)
                {
                    //log the error and continue to read for next feature class
                    _logger.Warn(tempFeatureClass.AliasName + ": doesnt contain FieldModelName:" + SchemaInfo.Electric.FieldModelNames.OperatingNumber + ", continuing.");
                    continue;
                }
                string tempOperatingNumberFieldName = tempFeatureClass.Fields.get_Field(tempOperatingNumberIdx).Name;
                int divisionIdx = -1, circuitIdIdx = -1;
                _logger.Debug("Executing GetIFeatureCursor");
                IFeatureCursor duplicateFeatures = GetIFeatureCursor(currentFeature, operatingNumberIdx, tempFeatureClass, tempOperatingNumberFieldName, out divisionIdx, out circuitIdIdx);
                _logger.Debug("Executed GetIFeatureCursor");
                try
                {
                    if (duplicateFeatures != null)
                    {
                        //atleast 1 duplicate feature found then report that error and break the loop to boost the performance
                        if (AddDuplicateErrorForAllFeatures(duplicateFeatures, tempFeatureClass.AliasName, circuitIdIdx, divisionIdx))
                        {
                            break;
                        }
                    }
                    else
                    {
                        _logger.Debug("Feature cursor is null, means either no duplicate features found or OID, Division and CircuitID fields not found.");
                    }
                }
                finally
                {
                    //if cursor is not null then release it
                    _logger.Debug("Releasing FeatureCursor.");
                    if (duplicateFeatures != null)
                    {
                        while (Marshal.ReleaseComObject(duplicateFeatures) > 0)
                        {
                        }
                    }
                    _logger.Debug("FeatureCursor released.");
                }
            }
            //check for duplicate features end
        }


        /// <summary>
        /// Adds Error for all features in passed cursor.
        /// </summary>
        /// <param name="duplicateFeatures">The FeatureCursor of duplicate features.</param>
        /// <param name="FeatureClassName">the FeatureClass name to be used in error.</param>
        /// <param name="divisionIdx">The Division field index.</param>
        /// <param name="circuitIdIdx">The CircuitId field index.</param>
        /// <returns>True if error recorded.</returns>
        private bool AddDuplicateErrorForAllFeatures(IFeatureCursor duplicateFeatures, string FeatureClassName, int circuitIdIdx, int divisionIdx)
        {
            bool errorRecorded = false;
            string tempErr;
            IFeature tempFeature = null;
            //add the duplicate operating number for multiple features error 
            while ((tempFeature = duplicateFeatures.NextFeature()) != null)
            {
                //make sure if atleast 1 error is reported then return true
                errorRecorded = true;
                string divisionDomainDesc = tempFeature.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.Division).Convert("<Null>");
                string circuitIdValue = tempFeature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.FeederID).Convert("<Null>");
                tempErr = string.Format(_err_Duplicate_Operating_Number, FeatureClassName, tempFeature.OID, divisionDomainDesc, circuitIdValue);
                _logger.Debug(tempErr);
                AddError(tempErr);
            }
            return errorRecorded;
        }

        ///// <summary>
        ///// Gives description or name of Coded value domain.
        ///// </summary>
        ///// <param name="feature">The feature</param>
        ///// <param name="idx">The Index</param>
        ///// <returns>The Coded value name/description.</returns>
        //private string GetDomainDesc(IFeature feature, int idx)
        //{
        //    string domainDesc = "";
        //    ICodedValueDomain dmain = feature.Fields.get_Field(idx).Domain as ICodedValueDomain;
        //    if (dmain != null)
        //    {
        //        if(!DBNull.Value.Equals(feature.get_Value(idx)))
        //        {
        //            int val = 0;
        //            if (int.TryParse(feature.get_Value(idx).ToString(), out val))
        //            {
        //                domainDesc = dmain.get_Name(val);
        //            }
        //        }
        //    }
        //    return domainDesc;
        //}

        /// <summary>
        /// Handles all rules from RulesTable for current feature.
        /// </summary>
        /// <param name="currentFeature">The CurrentFeature.</param>
        /// <param name="rule">The Rule.</param>
        /// <returns></returns>
        private List<string> HandleRulesFromRulesTable(IFeature currentFeature, Rule rule)
        {
            //clear all the errors we have already
            _err_List = null;

            bool isRequired = rule.Required == 1 ? true : false;

            //check for duplicate features start
            IFeatureClass currentFeatureClass = currentFeature.Class as IFeatureClass;
            if (currentFeatureClass == null)
            {
                //log the error and return the control
                _logger.Warn("CurrentFeatureClass is <Null>,exiting.");
                return _err_List;
            }
            //get the index of the field with model name 'SchemaInfo.Electric.FieldModelNames.OperatingNumber'
            int operatingNumberIdx = ModelNameFacade.FieldIndexFromModelName(currentFeatureClass, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
            if (operatingNumberIdx == -1)
            {
                //log the error and return the control
                _logger.Warn(currentFeatureClass.AliasName + ": doesn't contain FieldModelName " + SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                return _err_List;
            }

            string operatingNumber = "";
            if (CheckRequiredAndNull(currentFeature.get_Value(operatingNumberIdx), isRequired))
            {
                //log debug for OperatingNumber is null and required
                _logger.Debug("OperatingNumber is required but <Null>, exiting.");
                return _err_List;
            }
            else if (isRequired == false)
            {
                if (DBNull.Value.Equals(currentFeature.get_Value(operatingNumberIdx)))
                {
                    //Not required field and null so no need to check ahead
                    _logger.Debug("OperatingNumber is <Null> but not required, exiting.");
                    return _err_List;
                }
            }
            //Get the operating number value
            operatingNumber = currentFeature.get_Value(operatingNumberIdx) as string;
            _logger.Debug("OperatingNumber : " + operatingNumber);
            _logger.Debug("Executing CheckBeginsWithAndPaddedZero");
            //Check BeginsWith adn PaddedZero validation
            CheckBeginsWithAndPaddedZero(operatingNumber, rule.BeginsWith, currentFeatureClass.AliasName);
            _logger.Debug("Executed CheckBeginsWithAndPaddedZero");

            _logger.Debug("Executing CheckEndsWith");
            //Check for ends with
            CheckEndsWith(operatingNumber, rule.EndsWith);
            _logger.Debug("Executed CheckEndsWith");

            _logger.Debug("Executing CheckMinLenght");
            //Check for min length of operating number
            CheckMinLenght(operatingNumber, rule.MinLength);
            _logger.Debug("Executed CheckMinLenght");

            _logger.Debug("Executing CheckMaxLength");
            //Check for max length of operating number
            CheckMaxLength(operatingNumber, rule.MaxLength);
            _logger.Debug("Executed CheckMaxLength");

            _logger.Debug("Executing CheckOperatingNumberValidChars");
            //Check for valid chars (only Begins with and [0-9] digits)
            CheckOperatingNumberValidChars(operatingNumber, rule.BeginsWith.Trim());
            _logger.Debug("Executed CheckOperatingNumberValidChars");

            _logger.Debug("Executing CheckDuplicateFeatures");
            //Check for duplicate oprating number all across the featureclasses
            CheckDuplicateFeatures(currentFeature, operatingNumberIdx);
            _logger.Debug("Executed CheckDuplicateFeatures");

            return _err_List;
        }

        /// <summary>
        /// Adds error in common list.
        /// </summary>
        /// <param name="messsage">The Error message.</param>
        /// <returns>Total count of the number of Error messages logged.</returns>
        private int AddError(string messsage)
        {
            //check if err_list already initialized
            if (_err_List == null)
            {
                //initilize the err_list for the first time
                _err_List = new List<string>();
            }
            //add the err message
            _err_List.Add(messsage);
            return _err_List.Count;
        }
        #endregion Private
        #endregion OperatingNumber
    }
}
