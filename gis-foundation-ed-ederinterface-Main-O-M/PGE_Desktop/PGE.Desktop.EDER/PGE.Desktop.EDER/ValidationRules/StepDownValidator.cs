using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using log4net;
using System.Reflection;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validate the stepdown for certain "Z" field based on the connectioncode.
    /// </summary>
    [ComVisible(true)]
    [Guid("9982B026-9D61-4041-96D7-BA9BA9C77AD1")]
    [ProgId("PGE.Desktop.EDER.StepDownValidator")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class StepDownValidator : BaseValidationRule
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        #region Constructors
        public StepDownValidator()
            : base("PGE StepDown Validate \"Z\" Fields Based On ConnectionCode.", SchemaInfo.Electric.ClassModelNames.StepDown)
        {

        }
        #endregion Constructors

        #region Private function/Methodes

        /// <summary>
        /// Get the field index from row using fieldmodelname.
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="fieldModelName">Name of the field.</param>
        /// <returns>If found index of field else -1</returns>
        private int GetFieldIndex(IRow row, string fieldModelName)
        {
            int fieldIndex = -1;
            //Get the connectioncode field and its value of the current row.
            IObject obj = (IObject)row;
            IObjectClass objClass = obj.Class;
            fieldIndex = ModelNameFacade.FieldIndexFromModelName(objClass, fieldModelName);
            if (fieldIndex == -1)
            {
                _logger.Error("Field with model name (" + fieldModelName + ") not found in class (" + objClass.AliasName + ").");
            }
            return fieldIndex;
        }



        /// <summary>
        /// Validate the fields (ZLPCTIMP,ZHPCTIMP,ZHLPCTIMP,ZGBPCTIMP,ZTPPCTIMP) based on the connectioncode value and return the error messages.
        /// </summary>
        /// <param name="connectionCodeValue">Value of the connection code</param>
        /// <param name="row">Row</param>
        /// <returns>Error messages.</returns>
        private void ValidateZFields(int connectionCodeValue,IRow row)
        {
            const int ZL = 0;
            const int ZH = 1;
            const int ZHL = 2;
            const int ZGB = 3;
            const int ZT = 4;

            var fieldName = new[]
                {
                    "ZLPCTIMP",
                    "ZHPCTIMP",
                    "ZHLPCTIMP",
                    "ZGBPCTIMP",
                    "ZTPCTIMP"
                };
            Func<int, string> name = index => fieldName[index];

            var fieldModelName = new[]
                {
                    SchemaInfo.Electric.FieldModelNames.Zlpctimp,
                    SchemaInfo.Electric.FieldModelNames.Zhpctimp,
                    SchemaInfo.Electric.FieldModelNames.Zhlpctimp,
                    SchemaInfo.Electric.FieldModelNames.Zgbpctimp,
                    SchemaInfo.Electric.FieldModelNames.Ztpctimp
                };

            var fieldExists = new bool[fieldModelName.Length];
            var fieldValue = new double?[fieldModelName.Length];

            for (int i = 0; i < fieldModelName.Length; i++)
            {
                double? value;
                fieldExists[i] = TryGetValue(row, fieldModelName[i], out value);
                fieldValue[i] = value;
            }
            Func<int, bool> exists = index => fieldExists[index];
            Func<int, bool> isNullOrZero = index => fieldValue[index] == null || fieldValue[index] == 0.0;

            Func<string, string, string>
                joinResults = (shouldBeZero, shouldBeNonZero) =>
                              {
                                  if (shouldBeZero.Length > 0 && shouldBeNonZero.Length > 0)
                                  {
                                      return string.Format("{0} must be null or zero and {1} must not be null or zero.",
                                                              shouldBeZero,
                                                              shouldBeNonZero);
                                  }
                                  else if (shouldBeNonZero.Length == 0)
                                  {
                                      return string.Format("{0} must be null or zero.", shouldBeZero);
                                  }
                                  else if (shouldBeZero.Length == 0)
                                  {
                                      return string.Format("{0} must not be null or zero.", shouldBeNonZero);
                                  }
                                  return string.Empty;
                              };

            //case based on the connectioncode value.
            switch (connectionCodeValue)
            {
                // The field ZGBPCTIMP must be zero and ZHLPCTIMP or (ZHPCTIMP and ZLPCTIMP) must not be null or zero 
                case 3:
                case 5:
                case 8:
                    {
                        var errorBuilder = new StringBuilder();
                        var fields = new[] { ZHL, ZH, ZL };
                        if (fields.All(exists))
                        {


                            if (fields.All(isNullOrZero))
                            {
                                errorBuilder.Append("Either ZHLPCTIMP or both ZHPCTIMP and ZLPCTIMP must not be null or zero");//"Either ZHLPCTIMP or both ZHPCTIMP and ZLPCTIMP must be greater than zero");
                            }
                            else if (!fields.Any(isNullOrZero))
                            {
                                errorBuilder.Append("Either ZHLPCTIMP must not be null or zero or both ZHPCTIMP and ZLPCTIMP must not be null or zero");//Either ZHLPCTIMP must be zero or both ZHPCTIMP and ZLPCTIMP must be zero, but not all three");
                            }
                            // else if (!isNullOrZeroZHLPCTIMP || (!isNullOrZeroZHPCTIMP && !isNullOrZeroZLPCTIMP))
                            //      !a | (!b & !c) -> !a | !(b | c) -> !(a & (b | c))
                            else if ((isNullOrZero(ZHL) && (isNullOrZero(ZH) || isNullOrZero(ZL))))
                            {
                                errorBuilder.Append("Either ZHLPCTIMP or both ZHPCTIMP and ZLPCTIMP must not be null or zero");
                            }
                            else if ((!isNullOrZero(ZHL) && (!isNullOrZero(ZH) || !isNullOrZero(ZL))))
                            {
                                errorBuilder.Append("Either ZHLPCTIMP must not be null or zero or both ZHPCTIMP and ZLPCTIMP must not be null or zero");
                            }
                        }
                        // 1.	 ZGBPCTIMP must = null or zero
                        if (exists(ZGB) && !isNullOrZero(ZGB)) // Condition 1 (see component spec)
                        {
                            if (errorBuilder.Length > 0) errorBuilder.Append(" and ");
                            errorBuilder.Append("ZGBPCTIMP should be null or zero");
                        }
                        if (errorBuilder.Length > 0) { errorBuilder.Append("."); AddError(errorBuilder.ToString()); }
                        break;
                    }
                //ZTPPCTIMP and ZHLPCTIMP must be null or zero
                // ZHPCTIMP and ZLPCTIMP and ZGBPCTIMP must not = null or zero
                case 6:
                case 7:
                    {
                        var shouldBeZero = new[] { ZT, ZHL }
                                            .Where(i => exists(i) && !isNullOrZero(i))
                                            .Select(name)
                                            .Concatenate(", ");
                        var shouldBeNonZero = new[] { ZH, ZL, ZGB }
                                                .Where(i => exists(i) && isNullOrZero(i))
                                                .Select(name)
                                                .Concatenate(", ");
                        if (string.IsNullOrEmpty(shouldBeZero) && string.IsNullOrEmpty(shouldBeNonZero))
                            break;
                        AddError(joinResults(shouldBeZero, shouldBeNonZero));
                        break;
                    }
                //ZHLPCTIMP and ZGBPCTIMP must = null or zero
                //ZHPCTIMP  and ZLPCTIMP and ZTPCTIMP must not = null or zero
                case 9:
                case 10:
                    {
                        var shouldBeZero = new[] { ZHL, ZGB }
                                            .Where(i => exists(i) && !isNullOrZero(i))
                                            .Select(i => fieldName[i])
                                            .Concatenate(", ");
                        var shouldBeNonZero = new[] { ZH, ZL, ZT }
                                                .Where(i => exists(i) && isNullOrZero(i))
                                                .Select(i => fieldName[i])
                                                .Concatenate(", ");
                        if (string.IsNullOrEmpty(shouldBeZero) && string.IsNullOrEmpty(shouldBeNonZero))
                            break;
                        AddError(joinResults(shouldBeZero, shouldBeNonZero));
                        break;
                    }

            }
            // EDARCFM0386 - ArcFm shall ensure that ZT when populated must be between -20 and 20.
            // EDARCFM0387 - ArcFm shall ensure that ZH when populated must be between -20 and 20.
            // EDARCFM0388 - ArcFm shall ensure that ZL when populated must be between -20 and 20.
            // EDARCFM0385 - ArcFm shall ensure that ZHL when populated must be between 0 and 20.
            // EDARCFM0384 - ArcFm shall ensure that ZGB when populated must be between 0 and 20.
            {
                var mustBeInRange = new[]{ZT, ZH, ZL};
                foreach (var fieldIndex in mustBeInRange)
                {
                    if (fieldValue[fieldIndex] < -20 || fieldValue[fieldIndex] > 20)
                        AddError("Value of " + fieldName[fieldIndex] + " must be within -20 to 20.");
                }

                mustBeInRange = new[] {ZHL, ZGB};
                foreach (var fieldIndex in mustBeInRange)
                {
                    if (fieldValue[fieldIndex] < 0 || fieldValue[fieldIndex] > 20)
                        AddError("Value of " + fieldName[fieldIndex] + " must be within 0 to 20.");
                }
            }
        }

        private bool TryGetValue(IRow row, string fieldModelName, out double? result)
        {
            result = null;
            var objectClass = ((IObject)row).Class;
            int fieldIndex = ModelNameFacade.FieldIndexFromModelName(objectClass, fieldModelName);
            if (fieldIndex == -1)
            {
                _logger.Error("Field with model name (" + fieldModelName + ") not found in class (" + objectClass.AliasName + ").");
                return false;
            }
            _logger.Debug("(" + fieldModelName + ") field index = " + fieldIndex);

            var value = row.Value[fieldIndex];
            if (value is DBNull || value == null)
            {
                _logger.Debug("(" + fieldModelName + ") field value = <null>");
                return true;
            }
            double parseResult;
            string valueString = value.ToString();
            if (double.TryParse(valueString, out parseResult))
                result = parseResult;
            else
            {
                _logger.Debug("Field with model name (" + fieldModelName + ") field value is non-numeric");
                return false;
            }
            valueString = result.HasValue ? valueString : "<null>";
            _logger.Debug("(" + fieldModelName + ") field value = " + valueString);
            return true;
        }

        #endregion Private function/Methodes

        #region Base Validation Rule Overrides
        /// <summary>
        /// Validate the row.
        /// </summary>
        /// <param name="row"> The row data being validated. </param>
        /// <returns> A list of errors or nothing. </returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //If this rule is being filtered out - do not run it 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            
            //Get the connectioncode field and its value of the current row.

            int indexConnectionCode = GetFieldIndex(row, SchemaInfo.Electric.FieldModelNames.ConnectionCode);
            if (indexConnectionCode == -1)
            {
                _logger.Error("Missing field with model name (" + SchemaInfo.Electric.FieldModelNames.ConnectionCode + ").");
                return _ErrorList;
            }
            var connectionCodeObject = row.Value[indexConnectionCode];
            if (connectionCodeObject == DBNull.Value)
            {
                
                _logger.Error("ConnectionCode value is <Null>. (" + Name + ").");
                return _ErrorList;
            }

            int connectionCodeValue = int.Parse(connectionCodeObject.ToString());
            _logger.Debug("Connection code value = " + connectionCodeValue);

            ValidateZFields(connectionCodeValue, row);
            
            return _ErrorList;
        }
        /// <summary>
        /// Determines if the specified parameter is an object class that has been configured with a class model name identified
        /// in the _modelNames array.
        /// </summary>
        /// <param name="param">The object class to validate.</param>
        /// <returns>Boolean indicating if the specified object class has any of the appropriate model name(s).</returns>
        protected override bool EnableByModelNames(object param)
        {
            if (base.EnableByModelNames(param))
            {
                return true;
            }

            _logger.Warn(string.Format(WarnMissingModelNames, _ModelNames.Concatenate("\r\n")));
            return false;
        }
        #endregion Base Validation Rule Overrides

    }
}
