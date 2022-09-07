/*
 *PreLoad info
* insert into sde.telvent_validation_severitymap values (sde.gdb_util.next_rowid('SDE', 'telvent_validation_severitymap'),'PGE Validate Switchable Devices',0);
*commit;
*
*Check box in ArcFM Properties - Object Info - Rules for Transformer ...
 */
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
    /// Validates the conductors associated to a Switchable Device for Energizing standards.
    /// 
    /// </summary>
    [ComVisible(true)]
    [Guid("1D987850-A6CC-4743-9BF5-3E8C1CBF7983")]
    [ProgId("PGE.Desktop.EDER.ValidateSwitchableDevices")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateSwitchableDevices : BaseValidationRule
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

       
        private static readonly string _msgIdle = "Switchable Device {0} OID:{1} may not be Idle when CircuitID2 is populated.";
        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        #region Constructor

        /// <summary>
        /// Initilizes the instance of <see cref="ValidateSwitchableDevices"/> class.
        /// </summary>
        public ValidateSwitchableDevices()
            : base("PGE Validate Switchable Devices", SchemaInfo.Electric.ClassModelNames.Switchable)
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
        /// Validates the object.
        /// </summary>
        /// <param name="row">Row to be validated.</param>
        /// <returns>Returns the error list after validation.</returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            // A switchable device (DPD, Switch, OpenPoint, Fuse) where CIRCUITID2 IS NOT NULL cannot be in 'Idle' Status.
            IObject obj = row as IObject;
            string circuitid2 = obj.GetFieldValue("CIRCUITID2",false).ToString();
            string status = obj.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).ToString();

            if (obj == null) return _ErrorList;
            
            if (circuitid2.Length > 0 && status == "30")
                    AddError(string.Format(_msgIdle,obj.Class.AliasName, obj.OID));
                
            return _ErrorList;
        }

        #endregion Overridden BaseValidationRule Methods
    }
}
