using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using PGE.Desktop.EDER.ValidationRules.DateValidator;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("D6070A11-8772-417F-8F92-41EA98A15553")]
    [ProgId("PGE.Desktop.EDER.ValidateFaultIndicatorYearInstalled")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateFaultIndicatorInstalled : BaseValidationRule
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        // [FaultIndicatorYearInstalledAlias] of [FaultIndicaorYearInstalledValue] for [ClassAlias] [PrimaryDisplayFieldValue] is before 2000.
        const string ErrorBefore2000 = "{0} of {1} for {2} is before 2000.";

        //[FaultIndicatorYearInstalledAlias] of [FaultIndicaorYearInstalledValue] for [ClassAlias] [PrimaryDisplayFieldValue] is invalid. It is in the future.
        const string ErrorParadox = "{0} of {1} for {2} is invalid. It is in the future.";

        public ValidateFaultIndicatorInstalled()
            : base("PGE Validate Fault Indicator Year Installed", SchemaInfo.Electric.FieldModelNames.FaultIndicatorYearInstalled)
        { }

        protected override Miner.Interop.ID8List InternalIsValid(IRow row)
        {

            var obj = (IObject)row;

            var fieldYearInstalled = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.FaultIndicatorYearInstalled);
            if (fieldYearInstalled == null)
                return _ErrorList;

            var yearInstalled = fieldYearInstalled.Value.Convert(-1);
            if (yearInstalled == -1)
                return _ErrorList;

            if (yearInstalled < 2000)
                AddError(string.Format(ErrorBefore2000, fieldYearInstalled.Alias, yearInstalled, obj.Class.AliasName));
            if (yearInstalled > DateTime.Today.Year)
                AddError(string.Format(ErrorParadox, fieldYearInstalled.Alias, yearInstalled, obj.Class.AliasName));

            return _ErrorList;
        }
    }
}
