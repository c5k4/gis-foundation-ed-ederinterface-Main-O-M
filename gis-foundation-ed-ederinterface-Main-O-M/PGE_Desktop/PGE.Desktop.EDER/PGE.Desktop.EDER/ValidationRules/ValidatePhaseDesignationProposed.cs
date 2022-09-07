using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates the Phase Designation on Proposed Features.
    /// </summary>
    [ProgId("PGE.Desktop.EDER.ValidationRules.ValidatePhaseDesignationProposed")]
    [Guid("3D281E55-B26F-4A7F-91E8-8E5498E17B7E")]
    [ComVisible(true)]
    [Miner.ComCategories.ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidatePhaseDesignationProposed : BaseValidationRule
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static readonly string[] _modelNames = { SchemaInfo.Electric.FieldModelNames.PhaseDesignation };
        
        #region Constructor
        /// <summary>
        /// Initializes the instance of <see cref="ValidatePhaseDesignation"/>
        /// </summary>
        public ValidatePhaseDesignationProposed()
            : base("PGE Validate Phase Designation - Proposed", _modelNames)
        { }
        #endregion Constructor

        #region Overridden Methods
        /// <summary>
        /// Validates the feature for phase designation for proposed features which will be displayed as a warning.
        /// </summary>
        /// <param name="row">Instance of the feature to be validated</param>
        /// <returns>Returns the list of errors of the PhaseDesignation either null or empty</returns>
        /// 
        protected override Miner.Interop.ID8List InternalIsValid(IRow row)
        {
            try
            {
                // Only validate Proposed features.
                if (StatusHelper.IsProposed(row as IObject))
                {
                    List<string> errors = PhaseValidator.Validate(row);
                    _logger.Debug(string.Format("Validation complete received {0} errors.", errors.Count));
                    foreach (string error in errors)
                    {
                        _logger.Debug(error);
                        AddError(error);
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Phase Designation Proposed rule.", ex);
            }
            return base._ErrorList;
        }
        #endregion  Overridden Methods
    }
}
