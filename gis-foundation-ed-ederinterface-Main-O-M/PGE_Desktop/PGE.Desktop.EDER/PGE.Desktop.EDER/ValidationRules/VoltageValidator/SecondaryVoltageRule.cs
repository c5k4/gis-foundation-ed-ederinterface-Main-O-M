using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.ArcFM;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.EditorExt;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using log4net;

namespace PGE.Desktop.EDER.ValidationRules.VoltageValidators
{
    /// <summary>
    /// Enables the Secondary Voltage Validation rule and manages instances of the SecondaryVlotageValidator.
    /// </summary>
    [Guid("680AAC78-2BDB-4C11-BDCD-43DFD2B1F6FD")]
    [ProgId("PGE.Desktop.EDER.SecondaryVoltageRule")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class SecondaryVoltageRule : BaseValidationRule
    {
        #region Private
        /// <summary>
        /// Factory class for building validators.
        /// </summary>
        private VoltageValidatorFactory factory;

        /// <summary>
        /// Model names to make enabled this validation rule.
        /// </summary>
        private static string[] enabledModelNames = new string[] { "PGE_SECONDARYCONDUCTOR" };

        /// <summary>
        /// Model names for transformer feat classes.
        /// </summary>
        private static readonly string[] transformerModelNames = new string[] { "DISTRIBUTIONTRANSFORMER" };
        
        /// <summary>
        /// logger to log all the information, waning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryVoltageRule"/> class.
        /// </summary>
        public SecondaryVoltageRule()
            : base("PGE Validate Secondary Voltage", enabledModelNames)
        {
            // Get the Network Analysis extension and initialize the validator factory.
            //get the type of app ref
            _logger.Debug("Getting the type of esriFramework.AppRef.");
            Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
            //create instance of type app ref
            _logger.Debug("Creating instance of esriFramework.AppRef type.");
            object obj = Activator.CreateInstance(type);
            //cast the obj to IApplication
            _logger.Debug("Casting instance from Object to IApplication.");
            IApplication app = (IApplication)obj;
            //get the extension Utility Network Analyst
            _logger.Debug("Getting extension Utility Network Analyst.");
            INetworkAnalysisExt naExt = (INetworkAnalysisExt)app.FindExtensionByName("Utility Network Analyst");
            //create instance of VoltageValidatorFactory with extension Utility Network Analyst
            _logger.Debug("Creating instance of VoltageValidatorFactory with Utility Network Analyst extension.");
            factory = new VoltageValidatorFactory(naExt);
            _logger.Debug("Successfully Created instance of VoltageValidatorFactory with Utility Network Analyst extension.");
        }
        #endregion Constructor

        #region Override for validation rule
        /// <summary>
        /// Determines if the provided row is valid.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow row)
        {
            try
            {
                //Change to address INC000003803926 QA/QC freezing on the QAQC subtask 
                //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
                //    return _ErrorList; 

                // Validate Secondary Voltage
                _logger.Debug("Casting row as IFeature.");
                IFeature conductor = row as IFeature;

                // Validate Primary Voltage
                _logger.Debug("Create SecVoltageValidator object from factory.");
                SecVoltageValidator validator = factory.CreateSecondaryVoltageValidator(conductor, transformerModelNames);
                // Run the validator and add error messages to the base error list.
                _logger.Debug("Validating.");
                List<string> errors = validator.Validate();

                foreach (string error in errors)
                {
                    _logger.Debug(error);
                    AddError(error);
                }

            }
            catch (Exception ex)
            {
                _logger.Warn("Error occurred while validating Secondary voltage rule.", ex);
            }

            return base.InternalIsValid(row);

        }
        #endregion Override for validation rule
    }
}
