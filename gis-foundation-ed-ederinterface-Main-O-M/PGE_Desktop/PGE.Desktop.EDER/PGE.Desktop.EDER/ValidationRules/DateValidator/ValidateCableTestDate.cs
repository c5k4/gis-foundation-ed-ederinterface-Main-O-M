// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Editing-Dates - EDER Component Specification
// Shaikh Rizuan 10/10/2012	Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using log4net;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;


namespace PGE.Desktop.EDER.ValidationRules.DateValidator
{
    /// <summary>
    /// Validate InstallationDate and InstallJobYear field that is present on many feature Classes and objects.
    /// </summary>
    [ComVisible(true)]

    [Guid("68703BF3-2219-456E-A97A-231735A6A20B")]
    [ProgId("PGE.Desktop.EDER.ValidateCableTestDate")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateCableTestDate : BaseValidationRule
    {
        #region Class Variables
        private const int _minYear = 1970;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateCableTestDate(): base("PGE Validate Cable Test Date", SchemaInfo.Electric.FieldModelNames.TestDate)
        {
        }
        #endregion

        #region Base Validation Rule Overrides

         /// Validate the row.
        /// </summary>
        /// <param name="row"> The row data being validated with Generator table </param>
        /// <returns> A list of errors or nothing. </returns>
        /// 
        protected override ID8List InternalIsValid(IRow row)
        {

            IObject obj = row as IObject;
            string errorMsg = string.Empty;         
            DateTime currentDate = DateTime.Today; //current date
            int maxYear = currentDate.Year; // current year
            int testDateFieldIndx = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.TestDate);
            IField pField = ModelNameFacade.FieldFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.TestDate);
            
            if (testDateFieldIndx != -1)
            {
                if (obj.get_Value(testDateFieldIndx) != DBNull.Value && !string.IsNullOrEmpty(obj.get_Value(testDateFieldIndx).ToString()))
                {
                    bool minimumRangeYear = DateValidatorRule.MinimumRangeYear(obj, _minYear, SchemaInfo.Electric.FieldModelNames.TestDate, out errorMsg);
                   _logger.Debug(minimumRangeYear + " for " + _minYear + " of " +  SchemaInfo.Electric.FieldModelNames.TestDate);
                    if (!minimumRangeYear) // if false
                    {
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            AddError(errorMsg);
                        }
                    }
                    if (minimumRangeYear)//if true
                    {
                        bool maximumRangeYear = DateValidatorRule.MaximumRangeDate(obj, currentDate, SchemaInfo.Electric.FieldModelNames.TestDate, out errorMsg); //DateValidatorRule.MaximumRangeYear(obj, maxYear, SchemaInfo.Electric.FieldModelNames.TestDate, out errorMsg);
                        _logger.Debug(maximumRangeYear + " for " + currentDate + " of " + SchemaInfo.Electric.FieldModelNames.TestDate);
                        if (!maximumRangeYear)//if false
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
                    errorMsg = pField.AliasName + " value for " + obj.Class.AliasName + " is <Null> or empty";
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        AddError(errorMsg);
                    }
                }
            }
            else
            {
                
            }
            return _ErrorList;
        }
#endregion
    }
}
