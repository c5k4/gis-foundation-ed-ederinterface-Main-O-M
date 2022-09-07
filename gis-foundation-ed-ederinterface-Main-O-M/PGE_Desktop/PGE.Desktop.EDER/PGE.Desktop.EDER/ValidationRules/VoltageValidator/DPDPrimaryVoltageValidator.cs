using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

using log4net;

namespace PGE.Desktop.EDER.ValidationRules.VoltageValidators
{
    /// <summary>
    /// Validates the Operating Voltage of DynamicProtectiveDevice against its upstream conductors
    /// </summary>
    [ComVisible(true)]
    [Guid("21088E39-D89C-45D1-A139-5240F26BC59B")]
    [ProgId("PGE.Desktop.EDER.DPDPrimaryVoltageValidator")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class DPDPrimaryVoltageValidator : BaseValidationRule
    {
        #region Private Variables

        /// <summary>
        /// Logger to log the error/info
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static readonly string[] ConductorModelNames = new string[]
                                                               {
                                                                   SchemaInfo.Electric.ClassModelNames.PGEPriOHConductor,
                                                                   SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor,
                                                                   SchemaInfo.Electric.ClassModelNames.PGEBusBar
                                                               };

        private const string WarnMissingModelNames = "Missing object class or field model names:\r\n{0}";
        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes the instance of <see cref="DPDPrimaryVoltageValidator"/> class
        /// </summary>
        public DPDPrimaryVoltageValidator()
            : base("PGE Validate DPD Primary Voltage", SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice)
        // TODO: Consider using SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice instead. Will require changes to component spec, installation guide, etc.
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
        /// Validates the Operating Voltage of DynamicProtectiveDevice against its upstream conductors
        /// </summary>
        /// <param name="row">Row to validate</param>
        /// <returns>Returns the error list after validation</returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //If this rule is being filtered out - do not run it 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            var feature = row as IFeature;
            if (feature == null)
            {
                _logger.Debug("The row is not a feature.");
                return _ErrorList;
            }
            if (!(feature is ISimpleJunctionFeature))
            {
                _logger.Debug("The feature is not a SimpleJunctionFeature.");
                return _ErrorList;
            }

            IFeature upstreamEdge = null;
            try
            {
                //Get the First Upstream Edge
                upstreamEdge = TraceFacade.GetFirstUpstreamEdge(feature);
            }
            catch (MultipleUpstreamFeaturesException multipleUpstreamFeaturesException)
            {
                string message = multipleUpstreamFeaturesException.Message;
                AddError("The device has multiple upstream conductors connected.");
                return _ErrorList;
            }

            if (upstreamEdge == null)
            {
                _logger.Debug("No upstream features were traced.");
                return _ErrorList;
            }

            
            //Validate whether the Upstream Edge either of the defined Conductors
            if (!ModelNameFacade.ContainsClassModelName(upstreamEdge.Class, ConductorModelNames))
            {
                _logger.Debug(upstreamEdge.Class.AliasName + " is missing the following model name assignments:\r\n" + ConductorModelNames.Concatenate("\r\n"));
                return _ErrorList;
            }

            //Get the Operating Voltage of the Edge
            int edgeFieldIndex = ModelNameFacade.FieldIndexFromModelName(upstreamEdge.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
            int dpdFieldIndex = ModelNameFacade.FieldIndexFromModelName(feature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
            var errorMessage = string.Format("The feature is not inheriting Operating Voltage from the sourcing line ({0} OID: {1}).", upstreamEdge.Class.AliasName, upstreamEdge.OID);
            if (edgeFieldIndex == -1 || dpdFieldIndex == -1)
            {
                var logMessage = string.Format("No field with model name {0} exists on {1} or {2}.",
                                               SchemaInfo.Electric.FieldModelNames.OperatingVoltage,
                                               upstreamEdge.Class.AliasName,
                                               feature.Class.AliasName);
                _logger.Debug(logMessage);
                AddError(errorMessage);
                return _ErrorList;
            }

            object conductorOpVoltage = upstreamEdge.Value[edgeFieldIndex];
            if (conductorOpVoltage == null
                || conductorOpVoltage == DBNull.Value
                || !object.Equals(conductorOpVoltage, row.Value[dpdFieldIndex]))
            {
                AddError(errorMessage);
                return _ErrorList;
            }
            return _ErrorList;
        }

        #endregion Overridden BaseValidationRule Methods
    }
}
