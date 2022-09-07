using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

using PGE.Desktop.EDER.AutoUpdaters;

namespace PGE.Desktop.EDER.ValidationRules
{
    [Guid("8E650C30-FA24-424A-997E-28D0FA3BFC36")]
    [ProgId("PGE.Desktop.EDER.ValidateRelatedUnitRecordsError")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateRelatedUnitRecordsError : BaseValidationRule
    {

        #region Private Variables

        private static readonly string[] _enabledModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.PGETransformer,
            SchemaInfo.Electric.ClassModelNames.VoltageRegulator,
            SchemaInfo.Electric.ClassModelNames.StepDown,
            SchemaInfo.Electric.ClassModelNames.PGEUnitTable};

        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        
        #endregion Private Variables

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateRelatedUnitRecordsError()
            : base("PGE Validate Related Unit Records - Error", _enabledModelNames)
        {
        }
        #endregion Constructors

        #region Override for validation rule
        /// <summary>
        /// Determines if the provided row is valid.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            try
            {
                List<string> errors = UnitHelper.ValidateUnits(row);
                _logger.Debug(string.Format("Validation complete received {0} errors.", errors.Count));
                foreach (string error in errors)
                {
                    _logger.Debug(error);
                    AddError(error);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Related Unit Records Error rule.", ex);
            }
            return base.InternalIsValid(row);
        }
        #endregion
    }
}
