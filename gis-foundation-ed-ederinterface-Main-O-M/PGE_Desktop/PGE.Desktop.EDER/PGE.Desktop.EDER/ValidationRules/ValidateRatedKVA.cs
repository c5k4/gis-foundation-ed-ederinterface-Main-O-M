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

using PGE.Desktop.EDER.AutoUpdaters;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates RatedKVA & Amp Values for:
    ///  StepDowns, StepDownUnits,
    /// VoltageRegulators, VoltageRegulatorUnits.
    /// 
    /// </summary>
    [ComVisible(true)]
    [Guid("13E7832F-35CC-4364-ACA4-56BDFE8AF3E4")]
    [ProgId("PGE.Desktop.EDER.ValidateRatedKVA")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateRatedKVA : BaseValidationRule
    {
 			
       // 2 B&E - The RatedKVA attribute of all [Voltage Regulator] unit records must match the parent record.				
       // 2 B&E - Banked Voltage Regulators must have matching RatedKVA attribute across all Voltage Regulators.				
       // 3 C -The RatedAmps attribute of all Voltage Regulator unit records must match the parent record.				
       // 3 C - Banked Voltage Regulators must have matching RatedAmps attribute across all Voltage Regulators.				
       // 4 D - The sum of the RatedKVA attribute of StepDown unit records must match the parent record.		
         

        #region Private
        /// <summary>
        /// Constant error message.
        /// </summary>

        private static readonly string[] _modelNames = {     SchemaInfo.Electric.ClassModelNames.StepDown,
                                                    SchemaInfo.Electric.ClassModelNames.VoltageRegulator,
                                                    SchemaInfo.Electric.ClassModelNames.PGEUnitTable,
                                                    SchemaInfo.Electric.ClassModelNames.CapacitorBank};
        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private static string _msgVoltRatedKVA = "The RatedKVA attribute of all Voltage Regulator unit records must match the parent record. {0} OID: {1}.";
        private static string _msgVoltRatedAMPS = "The RatedAmps attribute of all Voltage Regulator unit records must match the parent record. {0}  OID: {1}.";
        private static string _msgVoltBankKVA = "Banked Voltage Regulators must have matching RatedKVA attribute across all Voltage Regulators. {0} OID: {1}.";
        private static string _msgVoltBankAMPS = "Banked Voltage Regulators must have matching RatedAmps attribute across all Voltage Regulators. {0}  OID: {1}.";
        private static string _msgStepDownKVA = "The sum of the RatedKVA attribute of StepDown unit records must match the parent record. {0}  OID: {1}.";

        private static string _msgCapKVA = "Standard field capacitor units are 100 kVAR, 200 kVAR, and 300 kVAR.";
        private static string _msgCapTotalKVA = "Field capacitors greater than 1800 kVAR total are not standard. Double-check NUMBEROFUNITS and UNITKVAR fields";

        #endregion Private

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateRatedKVA()
            : base("PGE Validate Unit KVA-Amps", _modelNames)
        {
            
        }
        #endregion Constructor

        #region Overrides for validation rule

        /// <summary>
        /// Determines if the provided row is valid.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            try
            {
                ShellInstance shellRecord = UnitHelper.CreateShellInstance(row);
                string tableName;
                if (shellRecord == null)
                {
                    tableName = (row as IObject).Class.AliasName;
                    int oid = row.OID;

                    _logger.Warn(String.Format("Shell Instance was unable to be created when when Validating Unit KVA-Amps. Class: {0}, OID: {1}.", tableName, oid));

                    return _ErrorList;
                }

                // Skip idle features.
                if (StatusHelper.IsIdle(shellRecord.Status))
                {
                    _logger.Info(string.Format("{0} OID: {1} has an Idle Status, Skipping Validating Unit KVA-Amps.", shellRecord.ClassAlias, shellRecord.OID));
                    return _ErrorList;
                }

                if (shellRecord.IsStepDown)
                {
                    ValidateStepDowns(shellRecord);
                }
                else if (shellRecord.IsVoltageRegulator)
                {
                    ValidateVoltageRegulators(shellRecord);
                }
                else if (shellRecord.IsCapacitorBank)
                {
                    ValidateCapacitorBanks(shellRecord);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Unit Kva-Amps rule.", ex);
            }

            return _ErrorList;
        }
        #endregion Overrides for validation rule

        /// <summary>
        /// Unit record(s) ratedkva sum is equal to stepdown features ratedkva.
        /// </summary>
        /// <param name="shell"></param>
        /// <returns>string</returns>
        private string ValidateStepDowns(ShellInstance shell)
        {
            _logger.Debug("ValidateStepDowns started.");
            string result = string.Empty;

            if (!GetUnitSum(shell))
            {
                if (shell.StartObject == ShellInstance.ShellorUnit.Shell)
                    AddError(string.Format(_msgStepDownKVA, shell.ClassAlias + "Unit", string.Join(",", shell.UnitRecords.Select(oid => oid.ObjectId.ToString()).ToArray())));
                else
                    AddError(string.Format(_msgStepDownKVA, shell.ClassAlias, shell.OID));
            }
            _logger.Debug("ValidateStepDowns ended.");
            return result;
        }

        /// <summary>
        /// Each voltageregulator unit records ratedkva&ratedamps is equal to the voltageregulator feature.
        /// </summary>
        /// <param name="shell"></param>
        /// <returns>string</returns>
        private string ValidateVoltageRegulators(ShellInstance shell)
        {
            // Need to check bank operatingnumber and status = other.
            // Shell value should be eq to each unit value.
            _logger.Debug("ValidateVoltageRegulators started.");
            string result = string.Empty;
            List<int> uoids = CompareEachUnitKVA(shell);


            if (uoids.Count > 0)
            {
                if (shell.StartObject == ShellInstance.ShellorUnit.Shell)
                    AddError(string.Format(_msgVoltRatedKVA, shell.ClassAlias + "Unit", string.Join(",", uoids.Select(oid => oid.ToString()).ToArray())));
                else
                    AddError(string.Format(_msgVoltRatedKVA, shell.ClassAlias, shell.OID));
            }

            uoids = CompareEachUnitAMPS(shell);
            if (uoids.Count > 0)
            {
                if (shell.StartObject == ShellInstance.ShellorUnit.Shell)
                    AddError(string.Format(_msgVoltRatedAMPS, shell.ClassAlias + "Unit", string.Join(",", uoids.Select(oid => oid.ToString()).ToArray())));
                else
                    AddError(string.Format(_msgVoltRatedAMPS, shell.ClassAlias, shell.OID));
            }
            // Check bank records.
            BankedInstance bank = UnitHelper.CheckforBankedVoltageRegulator(ref shell);
            if (shell.IsBankedVoltageRegulator)
            {
                _logger.Debug("Feature is a bank.");
                foreach (ShellInstance s in bank.ShellRecords)
                {
                    if (s.RatedAMP != shell.RatedAMP)
                        AddError(string.Format(_msgVoltBankAMPS, s.ClassAlias, s.OID.ToString()));
                    if (s.RatedKVA != shell.RatedKVA)
                        AddError(string.Format(_msgVoltBankKVA, s.ClassAlias, s.OID.ToString()));
                }
            }
            _logger.Debug("ValidateVoltageRegulators ended.");

            return result;
        }

        private string ValidateCapacitorBanks(ShellInstance shell)
        {
            // Capacitor.UnitKVAR value should be IN (100,200,300); else "Warning: Standard field capacitor units are 100 kVAR, 200 kVAR, and 300 kVAR"
            // If EDGIS.CAPACITOR.TOTALKVAR > 1800, then “Warning: Field capacitors greater than 1800 kVAR total are not standard. Double-check NUMBEROFUNITS and UNITKVAR fields”

            int[] validUnitKVARs = { 100, 200, 300 };
            int i;
            _logger.Debug("ValidateCapacitorBanks started.");
            string result = string.Empty;
            object obj = shell.Object.GetFieldValue("UNITKVAR",false);
            if (obj != null)
            {
                if (int.TryParse(obj.ToString(), out i))
                {
                    if (Array.IndexOf(validUnitKVARs, i) < 0)
                    {
                        AddError(_msgCapKVA);
                    }
                }
            }

            obj = shell.Object.GetFieldValue("TOTALKVAR",false);
            if (obj != null)
            {
                
                if (int.TryParse(obj.ToString(), out i))
                {
                    if (i > 1800)
                        AddError(_msgCapTotalKVA);
                }
            }

            _logger.Debug("ValidateCapacitorBanks ended.");

            return result;
        }
        /// <summary>
        /// Check each unit ratedkva value is equal feature value.
        /// </summary>
        /// <param name="shell"></param>
        /// <returns>List of features not equal.</returns>
        private List<int> CompareEachUnitKVA(ShellInstance shell)
        {
            List<int> result = new List<int>();
            foreach (UnitRecordInstance u in shell.UnitRecords)
            {
                if (shell.RatedKVA != u.RatedKVA)
                    result.Add(u.ObjectId);
            }
            return result;
        }

        /// <summary>
        /// Check each unit ratedamps value is equal feature value.
        /// </summary>
        /// <param name="shell"></param>
        /// <returns>List of features not equal.</returns>
        private List<int> CompareEachUnitAMPS(ShellInstance shell)
        {
            List<int> result = new List<int>();
            foreach (UnitRecordInstance u in shell.UnitRecords)
            {
                if (shell.RatedAMP != u.RatedAMP)
                    result.Add(u.ObjectId);
            }
            return result;
        }

        /// <summary>
        /// Checks ratedkva sum of unit records to feature.
        /// </summary>
        /// <param name="shell"></param>
        /// <returns>False if not equal, true if equal.</returns>
        private bool GetUnitSum(ShellInstance shell)
        {
            double unitSum = 0;

            foreach (UnitRecordInstance u in shell.UnitRecords)
            {
                unitSum += u.RatedKVA;
            }

            if (shell.RatedKVA == unitSum)
                return true;
            else
                return false;
        }
    }

}
