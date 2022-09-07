using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    [Guid("784ABC48-BF05-46F2-8770-CE70B231528E")]
    [ProgId("PGE.Desktop.EDER.ValidateHighSideConfiguration")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateHighSideConfiguration : BaseValidationRule
    {
        #region Private Variables

        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// 
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private static readonly string[] _enabledModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.PGETransformer,
            SchemaInfo.Electric.ClassModelNames.VoltageRegulator,
            SchemaInfo.Electric.ClassModelNames.CapacitorBank
        };
        private static readonly string[] _capBankModelNames = new string[] {

            SchemaInfo.Electric.ClassModelNames.CapacitorBank
        };

        private static readonly string[] _voltageRegulatorModelNames = new string[] {

            SchemaInfo.Electric.ClassModelNames.VoltageRegulator
        };

        private static readonly string _nominalVoltageFieldName = "NOMINALVOLTAGE";
        private static readonly string _transformerTypeFieldName = "TRANSFORMERTYPE";

        private static readonly string[] _threePhaseCompatibleValues = new string[] { "YG", "CD", "YU", "ZZ", "CDCG" };
        private static readonly string[] _three1PhaseUnitCompatibleValues = new string[] { "YG", "YU", "ZZ", "CDCG" };
        private static readonly string[] _three2PhaseUnitsCompatibleValues = new string[] { "CD", "YU", "ZZ", "CDCG" };

        #endregion Private Variables


        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateHighSideConfiguration()
            : base("PGE Validate HighSideConfiguration", _enabledModelNames)
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
                if (row == null)
                {
                    return _ErrorList;
                }

                IObject obj = (IObject)row;
                if (obj == null)
                {
                    string tableName = (row as IObject).Class.AliasName;
                    int oid = row.OID;
                    _logger.Warn(String.Format("Row could not be converted to an object when Validating HighSideConfiguration. Class: {0}, OID: {1}.", tableName, oid));
                    return _ErrorList;
                }
                // Skip idle features.
                if (StatusHelper.IsIdle(obj))
                {
                    _logger.Info(string.Format("{0} OID: {1} has an Idle Status, Skipping Validating HighSideConfiguration.", obj.Class.AliasName, obj.OID));
                    return _ErrorList;
                }
                if (IsCapacitorBank(row))
                {
                    // Capacitor Banks have separate logic based on the source line.
                    ValidateCapBankHighSideConfig(obj);
                }

                else
                {

                    // Get the Transformer or Voltage Regulator feature (shell) with the unit info and other metadata.
                    var shellRecord = UnitHelper.CreateShellInstance(row);

                    if (shellRecord == null)
                    {
                        string tableName = (row as IObject).Class.AliasName;
                        int oid = row.OID;
                        _logger.Warn(String.Format("Shell Instance was unable to be created when Validating HighSideConfiguration. Class: {0}, OID: {1}.", tableName, oid));
                        return _ErrorList;
                    }

                    // IField highsideConfigField = ModelNameFacade.FieldFromModelName(row.Table as IObjectClass, SchemaInfo.Electric.FieldModelNames.HighSideConfiguration);
                    if (IsVoltageRegulator(row))
                    {

                        // Voltage Regulator - determine if it's banked or single.
                        var bankedVoltageRegulator = UnitHelper.CheckforBankedVoltageRegulator(ref shellRecord);
                        if (bankedVoltageRegulator != null)
                        {
                            // Banked Voltage Regulators are 2 or 3 Voltage Regulator Features that have 
                            // the same CircuitID and OperatingNumber and are located on different poles.
                            // Banked Voltage Regulators should use each of the VoltageRegulator features as the
                            // units for the HighSideConfiguration logic.
                            shellRecord.IsBankedVoltageRegulator = true;
                            ValidateHighSideConfig(bankedVoltageRegulator, shellRecord, obj);
                            return _ErrorList;
                        }
                        else
                        {
                            // Single Votage Regulator Features should use the unit records to validate.
                            shellRecord.IsBankedVoltageRegulator = false;
                            ValidateHighSideConfig(shellRecord, obj);
                            return _ErrorList;
                        }
                    }
                    else
                    {
                        // Transformer.
                        shellRecord.IsBankedVoltageRegulator = false;
                        ValidateHighSideConfig(shellRecord, obj);
                        return _ErrorList;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating HighSideConfiguration rule.", ex);
            }
            return base.InternalIsValid(row);
        }

        #endregion Override for validation rule

        #region private methods
        /// <summary>
        /// Validate HighSideConfiguration for Banked Voltage Regulators.
        /// </summary>
        /// <param name="bank">Banked Voltage Regulator.</param>
        /// <param name="shell">Member of VoltageRegulator being validated.</param>
        /// <param name="field">HighSideConfiguration used to get Domain Values for message.</param>
        private void ValidateHighSideConfig(BankedInstance bank, ShellInstance shell, IObject obj)
        {

            int phase = bank.BankPhaseDesignationDescription.Length;
            string highside = shell.HighSideConfiguration;
            string highsideDesc = shell.HighSideConfigurationDescription;


            // -----------------------------------------------------------------------
            // Rule A - LG HIGHSIDECONFIG assigned to non single phase Transformer or VoltageRegulator.
            if (highside == "LG")
            {
                // 9-A.

                AddError(String.Format("HighSideConfiguration cannot be '{0}' for a Banked Voltage Regulator", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule B - LL HIGHSIDECONFIG assigned to 1 or 3 phase Transformer or VoltageRegulator.
            else if (highside == "LL")
            {
                // 11-B.

                AddError(String.Format("HighSideConfiguration cannot be '{0}' in a Banked Voltage Regulator", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule D - OY HIGHSIDECONFIG assigned to something other than 2 unit, 2 phase Transformer or VoltageRegulator with A/B style phasing on unit records.
            else if ((phase == 2) && (bank.ShellRecordCount == 2) && (highside != "OY"))
            {
                // 15-D.
                string highsideConfigExpectedDescr = obj.LookupDomainDescription("OY", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                AddError(String.Format("HighSideConfiguration must be '{0}' when a two phase Banked Voltage Regulator has two units present", highsideConfigExpectedDescr));
            }
            else if ((phase == 2) && (bank.ShellRecordCount != 2) && (highside == "OY"))
            {
                // 16-D.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when two units are not present in a Banked Voltage Regulator", highsideDesc));
            }
            else if ((phase != 2) && (highside == "OY"))
            {
                // 17-D.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when a Banked Voltage Regulator is not two phase", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule C - OD HIGHSIDECONFIG assigned to something other than 2 unit, 3 phase Transformer or VoltageRegulator with AB/BC style phasing on unit records.
            else if ((phase == 3) && (bank.ShellRecordCount == 2) && (highside != "OD"))
            {
                // 12-C.
                string highsideConfigExpectedDescr = obj.LookupDomainDescription("OD", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                AddError(String.Format("HighSideConfiguration must be '{0}' when a three phase Banked Voltage Regulator has two units present", highsideConfigExpectedDescr));
            }
            else if ((phase == 3) && (bank.ShellRecordCount != 2) && (highside == "OD"))
            {
                // 13-C.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when two units are not present in a Banked Voltage Regulator", highsideDesc));
            }
            else if ((phase != 3) && (highside == "OD"))
            {
                // 14-C.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when  a Banked Voltage Regulator is not three phase", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule E - (YG/CD/YU/ZZ/CDCG) HIGHSIDECONFIG assigned to 2 unit transformer or VoltageRegulator.
            else if ((bank.ShellRecordCount == 2) && (Array.IndexOf(_threePhaseCompatibleValues, highside) > -1))
            {
                // 18-E.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when a Banked Voltage Regulator has two units present", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule F - (YG/CD/YU/ZZ/CDCG) HIGHSIDECONFIG assigned to non-ABC transformer or VoltageRegulator.
            else if ((phase != 3) && (Array.IndexOf(_threePhaseCompatibleValues, highside) > -1))
            {
                //19-F

                AddError(String.Format("HighSideConfiguration cannot be '{0}' for a Banked Voltage Regulator that is not three phase", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule G - 3 unit YG  (ZZ YU CDCG) Transformer or VoltageRegulator has non- A/B/C unit record phasing. 
            else if ((phase == 3) && (bank.ShellRecordCount == 3) && (bank.Has3SinglePhaseUnits) && (Array.IndexOf(_three1PhaseUnitCompatibleValues, highside) < 0))
            {
                // 20-G.
                // string highsideConfigExpectedDescr = obj.LookupDomainDescription("YG", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                // AddError(String.Format("HighSideConfiguration must be '{0}' when a three phase Banked Voltage Regulator has three single phase units present", highsideConfigExpectedDescr));
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when a three phase Banked Voltage Regulator has three single phase units present", highsideDesc));
            }


            // -----------------------------------------------------------------------
            // Rule H - 3 unit CD  (ZZ YU CDCG) Transformer or VoltageRegulator has non- AB/BC/AC unit record phasing.
            else if ((phase == 3) && (bank.ShellRecordCount == 3) && (bank.Has3TwoPhaseUnits) && (Array.IndexOf(_three2PhaseUnitsCompatibleValues, highside) < 0))
            {
                // 23-H.
                //string highsideConfigExpectedDescr = obj.LookupDomainDescription("CD", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                //AddError(String.Format("HighSideConfiguration must be '{0}' when a three phase Banked Voltage Regulator has three two phase units present", highsideConfigExpectedDescr));
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when a three phase Banked Voltage Regulator has three two phase units present", highsideDesc));
            }



        }

        /// <summary>
        /// Validate HighSideConfiguration for Transformers and Single Voltage Regulators.
        /// </summary>
        /// <param name="shell">Transformer or Single VoltageRegulator</param>
        /// <param name="field">HighSideConfiguration used to get Domain Values for message</param>
        private void ValidateHighSideConfig(ShellInstance shell, IObject obj)
        {
            string highside = shell.HighSideConfiguration;
            string highsideDesc = shell.HighSideConfigurationDescription;
            int phase = shell.PhaseDesignationDescription.Length;

            // Used for Duplex Transformers.
            object[] unitTranTypes = { "21", "32" };
            string[] duplexTypes = { "OD", "OY" };
            bool isDuplex = false;

            object odebug = shell.UnitRecords[0].GetFieldValue(_transformerTypeFieldName);

            if ((Array.IndexOf(unitTranTypes, shell.UnitRecords[0].GetFieldValue(_transformerTypeFieldName)) >= 0))
            { 
                isDuplex = true;
            }

            // -----------------------------------------------------------------------
            // Rule A - LG HIGHSIDECONFIG assigned to non single phase Transformer or VoltageRegulator.
            if ((phase == 1) && (highside != "LG"))
            {

                // 8-A.
                string highsideConfigExpectedDescr = obj.LookupDomainDescription("LG", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                AddError(String.Format("HighSideConfiguration must be '{0}' when Phase Designation contains one phase", highsideConfigExpectedDescr));
            }
            else if ((phase != 1) && (highside == "LG"))
            {
                // 9-A.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when Phase Designation contains more than one phase", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule B - LL HIGHSIDECONFIG assigned to 1 or 3 phase Transformer or VoltageRegulator.
            else if ((phase == 2) && (!shell.HasOpenWyeUnitConfiguration) && (highside != "LL") && (shell.UnitRecords.Count() == 1) && !isDuplex)
            {
                // 10-B.
                string highsideConfigExpectedDescr = obj.LookupDomainDescription("LL", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                AddError(String.Format("HighSideConfiguration must be '{0}' when one unit is present and Phase Designation contains two phases", highsideConfigExpectedDescr));
            }
            else if ((phase != 2) && (highside == "LL"))
            {
                // 11-B. 
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when Phase Designation does not contain two phases", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule D - OY HIGHSIDECONFIG assigned to something other than 2 unit, 2 phase Transformer or VoltageRegulator with A/B style phasing on unit records.
            else if ((phase == 2) && (shell.UnitRecords.Count() == 2) && (highside != "OY"))
            {
                // 15-D.
                string highsideConfigExpectedDescr = obj.LookupDomainDescription("OY", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                AddError(String.Format("HighSideConfiguration must be '{0}' when two units are present and Phase Designation contains two phases", highsideConfigExpectedDescr));
            }
            else if ((phase == 2) && (shell.UnitRecords.Count() != 2) && (highside == "OY"))
            {
                // 16-D. 
                // Oringal message 10/26/2021.
                // AddError(String.Format("HighSideConfiguration cannot be '{0}' when two units are not present", highsideDesc));

                // Update BEC 10/26/2021.
                if (shell.UnitRecords.Count == 3)
                {
                    AddError(string.Format("HighSideConfiguration cannot be '{0}' when three units are present", highsideDesc));
                }
                else if ((shell.UnitRecords.Count == 1) && (Array.IndexOf(unitTranTypes,shell.UnitRecords[0].GetFieldValue(_transformerTypeFieldName)) < 0))
                {
                    // BEC Removed on 2/17/2022
                    // AddError(string.Format("HighSideConfiguration cannot be '{0}' when one unit is present on non-duplex transformer", highsideDesc));
                }
            }
            else if ((phase != 2) && (highside == "OY"))
            {
                // 17-D. 
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when Phase Designation does not contain two phases", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule C - OD HIGHSIDECONFIG assigned to something other than 2 unit, 3 phase Transformer or VoltageRegulator with AB/BC style phasing on unit records.
            else if ((phase == 3) && (shell.UnitRecords.Count() == 2) && (highside != "OD"))
            {
                // 12-C.
                string highsideConfigExpectedDescr = obj.LookupDomainDescription("OD", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                AddError(String.Format("HighSideConfiguration must be '{0}' when Phase Designation is three phase and two units are present", highsideConfigExpectedDescr));
            }
            else if ((phase == 3) && (shell.UnitRecords.Count() != 2) && (highside == "OD"))
            {
                // 13-C.
                // Oringal message 10/26/2021.
                // AddError(String.Format("HighSideConfiguration cannot be '{0}' when two units are not present", highsideDesc));

                // Update BEC 10/26/2021.
                if (shell.UnitRecords.Count == 3)
                {
                    AddError(string.Format("HighSideConfiguration cannot be '{0}' when three units are present", highsideDesc));
                }
                else if ((shell.UnitRecords.Count == 1) && (Array.IndexOf(unitTranTypes, shell.UnitRecords[0].GetFieldValue(_transformerTypeFieldName)) < 0))
                {
                    // BEC Removed on 2/17/2022
                 //     AddError(string.Format("HighSideConfiguration cannot be '{0}' when one unit is present on non-duplex transformer", highsideDesc));
                }
            }
            else if ((phase != 3) && (highside == "OD"))
            {
                // 14-C.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when Phase Designation is not three phase", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule E - (YG/CD/YU/ZZ/CDCG) HIGHSIDECONFIG assigned to 2 unit Transformer or VoltageRegulator.
            else if ((shell.UnitRecords.Count() == 2) && (Array.IndexOf(_threePhaseCompatibleValues, highside) > -1))
            {
                // 18-E.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when there are two associated unit records", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule F - (YG/CD/YU/ZZ/CDCG) HIGHSIDECONFIG assigned to non-ABC transformer or VoltageRegulator.
            else if ((phase != 3) && (Array.IndexOf(_threePhaseCompatibleValues, highside) > -1))
            {
                // 19-F.
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when Phase Designation is not three phase", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule G - 3 unit YG (ZZ YU CDCG) Transformer or VoltageRegulator has non- A/B/C unit record phasing.
            else if ((phase == 3) && (shell.UnitRecords.Count() == 3) && (shell.Has3SinglePhaseUnits) && (Array.IndexOf(_three1PhaseUnitCompatibleValues, highside) < 0))
            {
                // 20-G.
                //string highsideConfigExpectedDescr = obj.LookupDomainDescription("YG", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                //AddError(String.Format("HighSideConfiguration must be '{0}' when Phase Designation is three phase and contains (3) single phase unit records", highsideConfigExpectedDescr));
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when Phase Designation is three phase and contains (3) single phase unit records", highsideDesc));
            }
            // -----------------------------------------------------------------------
            // Rule H - 3 unit CD  (ZZ YU CDCG)  Transformer or VoltageRegulator has non- AB/BC/AC unit record phasing.
            else if ((phase == 3) && (shell.UnitRecords.Count() == 3) && (shell.Has3TwoPhaseUnits) && (Array.IndexOf(_three2PhaseUnitsCompatibleValues, highside) < 0))
            {
                // 23-H.
                //string highsideConfigExpectedDescr = obj.LookupDomainDescription("CD", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                //AddError(String.Format("HighSideConfiguration must be '{0}' when Phase Designation is three phase and contains (3) two phase unit records", highsideConfigExpectedDescr));
                AddError(String.Format("HighSideConfiguration cannot be '{0}' when Phase Designation is three phase and contains (3) two phase unit records", highsideDesc));
            }

            object sdebug = shell.UnitRecords[0].GetFieldValue(_transformerTypeFieldName);
            int idebug = Array.IndexOf(unitTranTypes, sdebug);
            idebug = Array.IndexOf(duplexTypes, highside);

            if ((shell.UnitRecords.Count == 1) && (Array.IndexOf(unitTranTypes, shell.UnitRecords[0].GetFieldValue(_transformerTypeFieldName)) >= 0) && (Array.IndexOf(duplexTypes,highside) < 0))
            {
                // Update BEC Duplex transformers
                AddError(string.Format("HighSideConfiguration cannot be '{0}' when one unit is present on duplex transformer", highsideDesc));
            }
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Validates Capacitor Bank HighSideConfiguration based on Sourcline Voltage and Neutral information.
        /// </summary>
        /// <param name="row">CapacitorBank Record</param>
        private void ValidateCapBankHighSideConfig(IRow row)
        {
            IObject obj = (IObject)row;
            string highsideValue = obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.HighSideConfiguration).Convert<string>(String.Empty);
            string highsideDesc = obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.HighSideConfiguration).Convert<string>(String.Empty);
            IFeature sourceline = TraceFacade.GetFirstUpstreamEdge(row as IFeature);

            if (highsideValue.IsNullOrEmpty())
            {
                AddError("HighSideConfiguration is null.");
            }
            else if (sourceline == null)
            {
                AddError("Cannot verify HighSideConfiguration: No SourceLine was found.");
            }
            else
            {
                int nominalVoltage = sourceline.GetFieldValue(useDomain: false, fieldName: _nominalVoltageFieldName).Convert<int>(-1);
                string nominalVoltageDesc = obj.GetFieldValue(fieldName: _nominalVoltageFieldName).Convert<string>(String.Empty);
                bool hasNeutral = NeutralPhaseHelper.Instance.HasNeutral(sourceline);

                // SourceLine Nominal Voltage = 21kV AND SourceLine has a neutral  AND HIGHSIDECONFIGUATION is not YG then INVALID.
                if ((nominalVoltage == 8) && (hasNeutral) && (highsideValue != "YG"))
                {
                    string expectedHighSideConfig = obj.LookupDomainDescription("YG", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                    AddError(String.Format("Capacitor Banks with a {0} sourceline with a Neutral must have a '{1}' HighSideConfiguration. Sourceline OID({2}) ", nominalVoltageDesc, expectedHighSideConfig, sourceline.OID));
                }
                // SourceLine Nominal Voltage = 12kV AND HIGHSIDECONFIGUATION is not CD then INVALID.
                else if ((nominalVoltage == 5) && (!hasNeutral) && (highsideValue != "CD"))
                {
                    string expectedHighSideConfig = obj.LookupDomainDescription("CD", SchemaInfo.Electric.FieldModelNames.HighSideConfiguration, String.Empty);
                    AddError(String.Format("Capacitor Banks with a {0} sourceline with no Neutral must have a '{1}' HighSideConfiguration. Sourceline OID({2}) ", nominalVoltageDesc, expectedHighSideConfig, sourceline.OID));

                }
            }

        }


        /// <summary>
        /// Determines if row being checked is a CapacitorBank. 
        /// </summary>
        /// <param name="row"></param>
        /// <returns>true/false</returns>
        private bool IsCapacitorBank(IRow row)
        {
            bool isCapBank = false;
            if (ModelNameFacade.ContainsClassModelName(row.Table as IObjectClass, _capBankModelNames))
            {
                isCapBank = true;

            }

            return isCapBank;
        }

        /// <summary>
        /// Determines if row being checked is a VoltageRegulator. 
        /// </summary>
        /// <param name="row"></param>
        /// <returns>true/false</returns>
        private bool IsVoltageRegulator(IRow row)
        {
            bool isVoltageRegulator = false;
            if (ModelNameFacade.ContainsClassModelName(row.Table as IObjectClass, _voltageRegulatorModelNames))
            {
                isVoltageRegulator = true;

            }

            return isVoltageRegulator;
        }

        #endregion private methods
    }
}