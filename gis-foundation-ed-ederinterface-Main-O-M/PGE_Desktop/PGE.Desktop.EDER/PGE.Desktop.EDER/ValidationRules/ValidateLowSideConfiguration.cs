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
    [Guid("B3193E1F-F487-48EF-960E-ABC55BFBAD93")]
    [ProgId("PGE.Desktop.EDER.ValidateLowSideConfiguration")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateLowSideConfiguration : BaseValidationRule
    {
        #region Private Variables

        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// 
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        // Static data used for validation.
        private static readonly string[] enabledModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PGETransformer };
        private static readonly string _transformerTypeFieldName = "TRANSFORMERTYPE";
        private static readonly string[] threePhaseCompatibleValues = new string[] { "YG", "CD", "OD", "CDCT", "CDCG" }; // Update BEC Removed "ODCT".
        private static readonly string[] threePhaseNonCompatibleValues = new string[] { "2W", "3W" };
        private static readonly string[] networkSubtypeCompatibleValues = new string[] { "YG" };
        private static readonly string[] compatibleValues120208 = new string[] { "YG", "OY", "ZZ", "YU" };
        private static readonly string[] compatibleValues120240_1Phase = new string[] { "2W", "3W" };
        private static readonly string[] compatibleValues120240_3Phase = new string[] { "CDCT", "ODCT", "CDCG" };
        private static readonly string[] compatibleValues240 = new string[] { "OD", "CD", "CDCT", "ODCT", "CDCG" };
        private static readonly string[] compatibleValues480 = new string[] { "OD", "CD", "CDCT", "ODCT", "CDCG" };
        private static readonly string[] compatibleValues277480 = new string[] { "YG", "OY", "ZZ", "YU" };
        private static readonly string[] compatibleValues240480 = new string[] { "CDCT", "ODCT", "CDCG" };
        private static readonly string[] compatibleValueHighOY = new string[] { "ODCT", "ZZ" };
        private static readonly string[] compatibleValueDuplex = new string[] { "ODCT" };

        private static readonly int networkSubtypeCode = 5;
        private static readonly string phaseErrorMessage = "Low Side Configuration of '{0}' is not compatible with a {1}-Phase Transformer.";
        private static readonly string lowSideVoltageErrorMessage = "Low Side Voltage of '{0}' is not compatible with a Low Side Configuration of '{1}'.";

        // Update BEC
        private static readonly string highSideVoltageErrorMessage = "Low Side Voltage of '{0}' is not compatible with a High Side Configuration of '{1}'.";
        private static readonly string highSideConfigErrorMessage = "Low Side Configuration of '{0}' is not compatible with a High Side Configuration of '{1}'.";
        private static readonly string duplexLowSideConfigMessage = "Low Side Configuration must be 'ODCT' for Duplex features.";
        private static readonly string duplexLowSideVoltageMessage = "Low Side Voltage must be '120/240 Three-Phase' for Duplex features.";
        // If duplex, then must have LowSideConfiguration = 'ODCT'
        // If duplex, then must have LowSideVoltage of '120/240 Three-Phase'
        private static readonly string lightLowSideVoltageMessage = "Low Side Voltage of 'StreetLight Only' must be on feature with Subtype 'Street Light'.";
        private static readonly string unitLowsideConfigMessage = "Low Side Configuration of {0} is not compatible when {1} units are present.";
        #endregion Private Variables


        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateLowSideConfiguration()
            : base("PGE Validate LowSideConfiguration", enabledModelNames)
        {
        }
        #endregion Constructors

        #region Override for validation rule
        /// <summary>
        /// Determines if the provided row is valid.
        /// </summary>
        /// <param name="row"> The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            try
            {
                if (row == null)
                {
                    return _ErrorList;
                }
                // Get values needed to Validate.
                IFeature tranformer = (IFeature)row;
                if (tranformer == null)
                {
                    string tableName = (row as IObject).Class.AliasName;
                    int oid = row.OID;
                    _logger.Warn(String.Format("Row could not be converted to a feature when Validating HighSideConfiguration. Class: {0}, OID: {1}.", tableName, oid));
                    return _ErrorList;
                }

                // Skip idle features.
                if (StatusHelper.IsIdle(tranformer))
                {
                    _logger.Info(string.Format("{0} OID: {1} has an Idle Status, Skipping Validating LowSideConfiguration.", tranformer.Class.AliasName, tranformer.OID));
                    return _ErrorList;
                }
                string phase = tranformer.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert<string>(String.Empty);
                int phaseLength = phase.Length;
                string lowsideConfig = tranformer.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.LowSideConfiguration).Convert<string>(String.Empty);
                string lowsideConfigDesc = tranformer.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.LowSideConfiguration).Convert<string>(String.Empty);
                int lowsideVoltage = tranformer.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.LowSideVoltage).Convert<int>(-1);
                string lowsideVoltageDesc = tranformer.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.LowSideVoltage).Convert<string>(String.Empty);
                string subtype = tranformer.GetSubtypeDescription();
                bool isNetworkTransformer = tranformer.HasSubtypeCode(networkSubtypeCode);

                // Update BEC
                string highsideConfig = tranformer.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.HighSideConfiguration).Convert<string>(String.Empty);
               

                // Error if not populated.
                if (lowsideConfig.IsNullOrEmpty())
                {
                    AddError("Low Side Configuration is null.");

                }
                // Errors if fields needed for validation are null.
                else if (phase.IsNullOrEmpty() || (lowsideVoltage == -1))
                {
                    if (phase.IsNullOrEmpty())
                    {
                        AddError("Low Side Configuration cannot be verified: Phase Designation is null.");
                    }
                    if (lowsideVoltage == -1)
                    {
                        AddError("Low Side Configuration cannot be verified: Low Side Voltage is null.");
                    }
                }
                // Check SubType first.
                //  Error C - Network transformer with non-YG LOWSIDECONFIG.
                else if (isNetworkTransformer && (Array.IndexOf(networkSubtypeCompatibleValues, lowsideConfig) < 0))
                {
                    //IField lowsideConfigField = ModelNameFacade.FieldFromModelName(tranformer.Class, SchemaInfo.Electric.FieldModelNames.LowSideConfiguration);
                    string lowsideConfigExpectedDescr = tranformer.LookupDomainDescription("YG", SchemaInfo.Electric.FieldModelNames.LowSideConfiguration, String.Empty);
                    AddError(String.Format("LowSideConfiguration for Network transformers must be '{0}'", lowsideConfigExpectedDescr));
                }
                // Check Phase Next.
                // Error A - YG/CD/OD/CDCT/ODCT/CDCG LOWSIDECONFIG assigned to non-three phase transformer. 
                else if ((!isNetworkTransformer && phaseLength != 3) && (Array.IndexOf(threePhaseCompatibleValues, lowsideConfig) > -1))
                {
                    AddError(String.Format(phaseErrorMessage, lowsideConfigDesc, phaseLength));

                }
                // Error B - 3W/2W LOWSIDECONFIG assigned to three phase transformer.
                else if ((!isNetworkTransformer && phaseLength == 3) && (Array.IndexOf(threePhaseNonCompatibleValues, lowsideConfig) > -1))
                {
                    AddError(String.Format(phaseErrorMessage, lowsideConfigDesc, phaseLength));
                }
                // Then check Lowsidevoltage Next.
                // Error D -120/208 LOWSIDEVOLTAGE assigned to non YG/OY/ZZ/YU LOWSIDECONFIG.
                else if ((!isNetworkTransformer && lowsideVoltage == 21) && (Array.IndexOf(compatibleValues120208, lowsideConfig) < 0))
                {
                    AddError(String.Format(lowSideVoltageErrorMessage, lowsideVoltageDesc, lowsideConfigDesc));
                }
                // Error E - 120/240 - Single Phase LOWSIDEVOLTAGE assigned to non 2W/3W LOWSIDECONFIG. 
                else if ((!isNetworkTransformer && lowsideVoltage == 22) && (Array.IndexOf(compatibleValues120240_1Phase, lowsideConfig) < 0))
                {
                    AddError(String.Format(lowSideVoltageErrorMessage, lowsideVoltageDesc, lowsideConfigDesc));
                }
                // Error F - 120/240 - Three Phase LOWSIDEVOLTAGE assigned to non CDCT/ODCT/CDCG LOWSIDECONFIG. 
                else if ((!isNetworkTransformer && lowsideVoltage == 23) && (Array.IndexOf(compatibleValues120240_3Phase, lowsideConfig) < 0))
                {
                    AddError(String.Format(lowSideVoltageErrorMessage, lowsideVoltageDesc, lowsideConfigDesc));
                }
                // Error G - 240 LOWSIDEVOLTAGE assigned to non OD/CD/CDCT/ODCT/CDCG LOWSIDECONFIG.
                else if ((!isNetworkTransformer && lowsideVoltage == 25) && (Array.IndexOf(compatibleValues240, lowsideConfig) < 0))
                {
                    AddError(String.Format(lowSideVoltageErrorMessage, lowsideVoltageDesc, lowsideConfigDesc));
                }
                // Error H - 480 LOWSIDEVOLTAGE assigned to non OD/CD/CDCT/ODCT/CDCG LOWSIDECONFIG.
                else if ((!isNetworkTransformer && lowsideVoltage == 27) && (Array.IndexOf(compatibleValues480, lowsideConfig) < 0))
                {
                    AddError(String.Format(lowSideVoltageErrorMessage, lowsideVoltageDesc, lowsideConfigDesc));
                }
                // Error I - 277/480 LOWSIDEVOLTAGE assigned to non YG/OY/ZZ/YU LOWSIDECONFIG.
                else if ((!isNetworkTransformer && lowsideVoltage == 26) && (Array.IndexOf(compatibleValues277480, lowsideConfig) < 0))
                {
                    AddError(String.Format(lowSideVoltageErrorMessage, lowsideVoltageDesc, lowsideConfigDesc));
                }
                // Error J - 240/480 LOWSIDEVOLTAGE assigned to non CDCT/ODCT/CDCG  LOWSIDECONFIG.
                else if ((!isNetworkTransformer && lowsideVoltage == 24) && (Array.IndexOf(compatibleValues240480, lowsideConfig) < 0))
                {
                    AddError(String.Format(lowSideVoltageErrorMessage, lowsideVoltageDesc, lowsideConfigDesc));
                }

                // Update 10/14/2021 BEC
                if ((highsideConfig == "OY") && (Array.IndexOf(compatibleValueHighOY, lowsideConfig) < 0))
                {
                    AddError(String.Format(highSideConfigErrorMessage, lowsideConfig, highsideConfig));
                }

                var shellRecord = UnitHelper.CreateShellInstance(row);
                if (shellRecord != null)
                {
                    UnitValidation(shellRecord, lowsideConfig, lowsideVoltage, highsideConfig);
                    StreetLightValidation(shellRecord, lowsideVoltage);
                } 

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating LowSideConfiguration rule.", ex);
            }
            return base.InternalIsValid(row);
        }



        #endregion Override for validation rule

        private void UnitValidation(ShellInstance shellRecord, string lowsideConfig, int lowsideVoltage, string highsideConfig)
        {
            // Used for Duplex Transformers.
            //int[] unitTranTypes = { 21, 32 };
            //bool isDuplex = false;


            // Duplex transformer rules.
            if ((shellRecord.IsDuplex))
            {
                if (lowsideConfig != "ODCT")
                    AddError(duplexLowSideConfigMessage);

                // If duplex, then must have LowSideVoltage of '120/240 Three-Phase' (23).
                if (lowsideVoltage != 23)
                    AddError(duplexLowSideVoltageMessage);

            }

            if (highsideConfig == "OY" && (lowsideConfig != "ZZ" || lowsideConfig != "ODCT"))
            {
                return;
            }
            // Update of LowSide QA-QC to account for unit count logic(e.g.OD, OY, ODCT shouldn’t be assigned to 1 or 3 unit transformer;
            // CD, YG, etc.shouldn’t be assigned to 2 unit transformer)
            string[] notTwoUnits = { "OD", "OY", "ODCT" };

            if ((shellRecord.IsDuplex))
            {
                // Duplex ODCT is fine.
                notTwoUnits = new string[] { "OD","OY"};
            }

            if ((shellRecord.UnitRecords.Count == 2))
            {
                if ((Array.IndexOf(notTwoUnits, lowsideConfig) < 0)) // || (lowsideConfig != "ZZ"))
                {
                    AddError(string.Format(unitLowsideConfigMessage, lowsideConfig, shellRecord.UnitRecords.Count));
                }
            }
            else
            {
                if (Array.IndexOf(notTwoUnits, lowsideConfig) >= 0)
                {
                    AddError(string.Format(unitLowsideConfigMessage, lowsideConfig, shellRecord.UnitRecords.Count));
                }
            }
        }
        /// <summary>
        /// Validate Street Light Voltage.
        /// </summary>
        /// <param name="shellRecord"></param>
        private void StreetLightValidation(ShellInstance shellRecord, int lowsideVoltage)
        {
         
            // 7 is Street Light.
            if (shellRecord.SubTypeCode == 7)
            {
                if (lowsideVoltage != 20)
                {
                    AddError(lightLowSideVoltageMessage);
                }
            }
            else
            {
                if (lowsideVoltage == 20)
                {
                    AddError(lightLowSideVoltageMessage);
                }
            }
        }
    }

}
