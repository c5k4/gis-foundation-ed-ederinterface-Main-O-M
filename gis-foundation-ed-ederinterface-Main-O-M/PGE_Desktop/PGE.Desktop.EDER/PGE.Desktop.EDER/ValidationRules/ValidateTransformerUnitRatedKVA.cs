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
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    
    /// <summary>
    /// Validates RatedKVA Values for:
    /// TransformerUnits.
    /// </summary>
    [ComVisible(true)]
    [Guid("07CEFD41-C7D1-4126-BBC6-D3BA3932A28A")]
    [ProgId("PGE.Desktop.EDER.ValidateTransformerUnitRatedKVA")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateTransformerUnitRatedKVA : BaseValidationRule
    {

        // 1A - Transformer unit record RatedKVA attribute cannot be less than 1 for non-Equipment transformers.


        #region Private Variables

        private static readonly string[] _modelNames = {     SchemaInfo.Electric.ClassModelNames.PGETransformer,
                                                    SchemaInfo.Electric.ClassModelNames.TransformerUnit};
        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// Constant error message.
        /// </summary>
        private static readonly string _msgNonEquipTX = "Transformer unit record RatedKVA attribute cannot be less than 1 for non-Equipment transformers.{0} OID: {1}";

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateTransformerUnitRatedKVA()
            : base("PGE Validate Transformer Unit RatedKVA", _modelNames)
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

                    _logger.Warn(String.Format("Shell Instance was unable to be created when Validating TransformerUnit RatedKVA. Class: {0}, OID: {1}.", tableName, oid));

                    return _ErrorList;
                }

                // Skip idle features.
                if (StatusHelper.IsIdle(shellRecord.Status))
                {
                    _logger.Info(string.Format("{0} OID: {1} has an Idle Status, Skipping Validating TransformerUnit RatedKVA.", shellRecord.ClassAlias, shellRecord.OID));
                    return _ErrorList;
                }


                if (shellRecord.IsTransformer)
                {
                    _logger.Debug("Feature is a transformer.");
                    ValidateTransformers(shellRecord);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Transformer Unit RatedKva rule.", ex);
            }
            return _ErrorList;

        }
        #endregion Overrides for validation rule

        /// <summary>
        /// Checks transformer feature ratedkva value against related unit records.
        /// </summary>
        /// <param name="shell"></param>
        /// <returns>string</returns>
        private string ValidateTransformers(ShellInstance shell)
        {
            // Shell should be equal sum of units.
            string result = string.Empty;

            try
            {
                _logger.Debug("ValidateTransformers started.");
                // Subtype 4 is equipment, just want distribution types.
                if ((shell.SubTypeCode == 1) || (shell.SubTypeCode == 2) || (shell.SubTypeCode == 3))
                {
                    _logger.Debug("Is correct subtype.");
                    IsLessThenOne(shell);
                }
                _logger.Debug("ValidateTransformers ended.");
                return result;
            }
            catch (System.Exception ex)
            {
                _logger.Error("ERROR ValidateTransformer " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                return result;
            }
        }

        /// <summary>
        /// Check if ratedkva is less then 1.
        /// </summary>
        /// <param name="shell"></param>
        /// <returns></returns>
        private bool IsLessThenOne(ShellInstance shell)
        {
            bool result = false;

            foreach (UnitRecordInstance u in shell.UnitRecords)
            {
                if (u.RatedKVA < 1)
                {
                    if (shell.StartObject == ShellInstance.ShellorUnit.Shell)
                        AddError(string.Format(_msgNonEquipTX, shell.ClassAlias + "Unit", string.Join(",", shell.UnitRecords.Select(x => x.ObjectId.ToString()).ToArray())));
                    else
                        AddError(string.Format(_msgNonEquipTX, shell.ClassAlias, shell.OID));

                    result = true;
                }
            }

            return result;
        }

    }
}