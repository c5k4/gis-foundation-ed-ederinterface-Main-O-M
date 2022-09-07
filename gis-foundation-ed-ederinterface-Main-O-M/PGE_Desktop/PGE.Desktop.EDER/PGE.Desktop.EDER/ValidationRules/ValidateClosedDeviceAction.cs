using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

using log4net;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates the conductors associated to a Switchable Device for Energizing standards
    /// </summary>
    [ComVisible(true)]
    [Guid("F87312E0-7E32-4EAA-89DE-2F20A52CFBF2")]
    [ProgId("PGE.Desktop.EDER.ValidateClosedDeviceAction")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateClosedDeviceAction : BaseValidationRule
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");


        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        #region Constructor

        /// <summary>
        /// Initilizes the instance of <see cref="ValidateClosedDeviceAction"/> class
        /// </summary>
        public ValidateClosedDeviceAction()
            : base("PGE Validate Closed Device Action", SchemaInfo.Electric.ClassModelNames.Switchable)
        { }

        #endregion Constructor

        #region Overridden BaseValidationRule Methods

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

        /// <summary>
        /// Validates the object for the Enerigized standards
        /// </summary>
        /// <param name="row">Row to be validated</param>
        /// <returns>Returns the error list after validation</returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //If this rule is being filtered out - do not run it 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            //Validate input row
            IObject obj = row as IObject;
            if (obj == null) return _ErrorList;

            //Validate the Object with Closed Status
            if (ValidateClosedDevice.IsDeviceClosed(obj, false))
            {
                string errorMessage;
                //If the Feature is invalid then add the error message
                if (ValidateClosedDevice.IsValid(obj, out errorMessage) == false)
                {
                    AddError(errorMessage);
                }
            }
            return _ErrorList;
        }

        #endregion Overridden BaseValidationRule Methods
    }
}
