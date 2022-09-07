// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Status-based Field Required - EDER Component Specification
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
    /// Validate Year Manufactured field that is present on many feature Classes.
    /// </summary>
    [ComVisible(true)]
    [Guid("B2E78A04-1242-4384-ABA8-5A383470E8D7")]
    [ProgId("PGE.Desktop.EDER.ValidateYearManufactured")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateYearManufactured : BaseValidationRule
    {
        #region Class Variables
        private const int _minYear = 1900;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        #endregion
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateYearManufactured()
            : base("PGE Validate Year Manufactured", SchemaInfo.Electric.FieldModelNames.ManufacturedYear)
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
            var today = DateTime.Today; //current date

            var fieldManufactureYear = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.ManufacturedYear);
            if (fieldManufactureYear != null)
            {
                var manufactureYear = fieldManufactureYear.Value.Convert<int>(0);
                if (manufactureYear == 0)
                    return _ErrorList;

                //INC000004009376 - no need to validate this condition since it is already validated by 
                //PGE Validate ESRI Field Rules when it validates the Range Domain: 'Year' 
                //if (manufactureYear < _minYear)
                //    AddError(fieldManufactureYear.Alias + " of " + manufactureYear + " is before " + _minYear + ".");

                if (manufactureYear > today.Year)
                    AddError(fieldManufactureYear.Alias + " of " + manufactureYear + " is invalid. It is in the future.");

                /*
                var fieldInstallYear = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.InstallationJobYear);
                if (fieldInstallYear != null)
                {
                    var installYear = fieldInstallYear.Value.Convert<int>(0);
                    if (installYear != 0 && manufactureYear > installYear)
                        AddError(fieldManufactureYear.Alias + " must be less than " + fieldInstallYear.Alias + ".");
                }*/

                var fieldInstallDate = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.InstallationDate);
                if (fieldInstallDate != null)
                {
                    var installDate = fieldInstallDate.Value.Convert<DateTime>(DateTime.MinValue);
                    if (installDate != DateTime.MinValue && manufactureYear > installDate.Year)
                        AddError(fieldManufactureYear.Alias + " must be less than " + fieldInstallDate.Alias + ".");
                }
            }

            return _ErrorList;
        }
        #endregion
    }
}
