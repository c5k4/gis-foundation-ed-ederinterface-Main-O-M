// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Status-based Field Required - EDER Component Specification
// Shaikh Rizuan 10/10/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules.DateValidator
{
    /// <summary>
    /// Validate InstallationDate and InstallJobYear field that is present on many feature Classes and objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("2F52559C-F5F1-40DA-BFA8-D020F72FEE62")]
    [ProgId("PGE.Desktop.EDER.ValidateWindSpeedRerateYear")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateWindSpeedRerateYear : BaseValidationRule
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const int _minYear = 1990;

        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateWindSpeedRerateYear()
            : base("PGE Validate Wind Speed Rerate Year", SchemaInfo.Electric.FieldModelNames.WindSpeedRerateYr)
        {
        }
        #endregion
        #region Base Validation Rule Overrides
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
         /// Validate the row.
        /// </summary>
        /// <param name="row"> The row data being validated with Generator table </param>
        /// <returns> A list of errors or nothing. </returns>
        /// 
        protected override ID8List InternalIsValid(IRow row)
        {

            IObject obj = row as IObject;
            string errorMsg = string.Empty;
            DateTime currentDate = DateTime.Today; // current date
            int maxYear = currentDate.Year; //cureent year
            int windSpeedCodeFldIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.WindSpeedCode);
            int windSpeedRerateYrFldIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.WindSpeedRerateYr);


            //check if model name saaigned
            if (windSpeedCodeFldIndex != -1 && windSpeedRerateYrFldIndex != -1)
            {
                object windSpeedCodeValue = obj.get_Value(windSpeedCodeFldIndex);
                object windSpeedRerateValue = obj.get_Value(windSpeedRerateYrFldIndex);

                //check for if field value is not null or empty
                if (windSpeedCodeValue != System.DBNull.Value && !string.IsNullOrEmpty(windSpeedCodeValue.ToString()))
                {
                    int val = Convert.ToInt32(windSpeedCodeValue);
                    if (val == 2)
                        return _ErrorList;

                    //check for if field value is null
                    if (windSpeedRerateValue == System.DBNull.Value && string.IsNullOrEmpty(windSpeedRerateValue.ToString()))
                    {
                        errorMsg = "Wind Speed Rerate Year field value is missing. It must be supplied when Wind Speed Rate is not 2 ft. /sec.";
                        AddError(errorMsg);
                        return _ErrorList;
                    }
                    bool minimumRangeYear = DateValidatorRule.MinimumRangeYear(obj, _minYear, SchemaInfo.Electric.FieldModelNames.WindSpeedRerateYr, out errorMsg);
                    _logger.Debug(minimumRangeYear + " for " + _minYear);

                    if (minimumRangeYear)
                    {
                        bool maximumRangeYear = DateValidatorRule.MaximumRangeYear(obj, maxYear, SchemaInfo.Electric.FieldModelNames.WindSpeedRerateYr, out errorMsg);
                        _logger.Debug(maximumRangeYear + " for " + maxYear);
                        if (!maximumRangeYear && !string.IsNullOrEmpty(errorMsg))//if false
                        {
                            AddError(errorMsg);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            AddError(errorMsg);
                        }
                    }
                }
            }
            else
            {
                _logger.Warn("Verify that Model name " + SchemaInfo.Electric.FieldModelNames.WindSpeedCode + " assigned to the fields properly.");
            }
            return _ErrorList;
        }
#endregion

    }
}
