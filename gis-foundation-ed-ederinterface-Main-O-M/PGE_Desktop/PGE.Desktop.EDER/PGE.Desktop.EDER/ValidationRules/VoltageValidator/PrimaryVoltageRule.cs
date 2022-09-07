using System;
using System.Runtime.InteropServices;

using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Framework;
using System.Collections.Generic;
using PGE.Common.Delivery.Framework;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using log4net;

namespace PGE.Desktop.EDER.ValidationRules.VoltageValidators
{
    /// <summary>
    /// Primary Voltage Validation Rule adds the voltage validator class to the ArcFM UI and manages instances of the PrimaryVoltageValidator class.
    /// </summary>
    [Guid("1E1692CC-F91A-4417-A3C0-49E0AA75568D")]
    [ProgId("PGE.Desktop.EDER.PrimaryVoltageRule")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class PrimaryVoltageRule : BaseValidationRule
    {
        #region Private
        // Q1 -2021 QA/QC phase rule change for ED-GIS Scripting project, enable for any with Operating Voltage attribute.
        // Will only be configured to run on primary features for phase 1 of ADMS project.
        // private static string[] enabledModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryConductor, SchemaInfo.Electric.ClassModelNames.StepDown, SchemaInfo.Electric.ClassModelNames.VoltageRegulator };
        private static string[] enabledModelNames = { SchemaInfo.Electric.FieldModelNames.OperatingVoltage4PrimaryValidation };
        /// <summary>
        /// Local IApplicaiton object.
        /// </summary>
        private IApplication app;

        /// <summary>
        /// Network Analyst Extension for tracing.
        /// </summary>
        private INetworkAnalysisExt naExt;

        /// <summary>
        /// Factory class for creating new voltage validates.
        /// </summary>
        private VoltageValidatorFactory factory;

        /// <summary>
        /// logger to log all the information, waning and errors.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryVoltageRule"/> class.
        /// </summary>
        public PrimaryVoltageRule()
            : base("PGE Validate Primary Voltage", enabledModelNames)
        {
            // Get the type of app ref.
            _logger.Debug("Getting the type of esriFramework.AppRef.");
            Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
            // Create instance of type app ref.
            _logger.Debug("Creating instance of esriFramework.AppRef type.");
            object obj = Activator.CreateInstance(type);
            // Cast the obj to IApplication.
            _logger.Debug("Casting instance from Object to IApplication.");
            app = (IApplication)obj;
            // Get the extension Utility Network Analyst.
            _logger.Debug("Getting extension Utility Network Analyst.");
            naExt = (INetworkAnalysisExt)app.FindExtensionByName("Utility Network Analyst");
            // Create instance of VoltageValidatorFactory with extension Utility Network Analyst.
            _logger.Debug("Creating instance of VoltageValidatorFactory with Utility Network Analyst extension.");
            factory = new VoltageValidatorFactory(naExt);
            _logger.Debug("Successfully Created instance of VoltageValidatorFactory with Utility Network Analyst extension.");
        }
        #endregion Constructor

        #region Override for validation rule
        /// <summary>
        /// Execute validation rule on the provided record.  Returns a list of errors.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow row)
        {

            try
            {
                // Change to address INC000003803926 QA/QC freezing on the QAQC subtask.
                // if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
                //    return _ErrorList; 

                // Validate Primary Voltage.
                _logger.Debug("Create PriVoltageValidator object from factory.");
                PriVoltageValidator validator = factory.CreatePrimaryVoltageValidator(row as IFeature);
                if (validator != null)
                {
                    _logger.Debug("Successfully created instance of PriVoltageValidator object from factory.");
                    // Run the validator and add error messages to the base error list.
                    _logger.Debug("Validating.");
                    List<string> errors = validator.Validate(base.Severity);
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
                _logger.Warn("Error occurred while validating Primary voltage rule.", ex);
            }
            
            return base.InternalIsValid(row);
        }
        #endregion Override for validation rule
    }
}
