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

    [Guid("12C288DA-4E08-4A80-ACEA-D2B26A3E49A2")]
    [ProgId("PGE.Desktop.EDER.ValidateRelatedUnitRecords")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateRelatedUnitRecords : BaseValidationRule
    {
        #region Private Variables

        private static readonly string[] _enabledModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.PGETransformer,
            SchemaInfo.Electric.ClassModelNames.VoltageRegulator,
            SchemaInfo.Electric.ClassModelNames.StepDown,
            SchemaInfo.Electric.ClassModelNames.PGEUnitTable};

        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// 
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        
        #endregion Private Variables

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateRelatedUnitRecords()
            : base("PGE Validate Related Unit Records", _enabledModelNames)
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
                List<string> errors = UnitHelper.ValidateUnits(row);
                _logger.Debug(string.Format("Validation complete received {0} errors.", errors.Count));

                string rowName = ((row as IObject).Class as IDataset).Name.ToUpper();

                foreach (string error in errors)
                {
                    _logger.Debug(error);
                    // Update BEC  •	Move warning message from Unit to Feature for Transformers and Voltage Regulators.
                    if (rowName == "EDGIS.TRANSFORMERUNIT" || rowName == "EDGIS.VOLTAGEREGULATORUNIT")
                    {
                        // Swapping unit row for feature to place errors there
                        row = RelatedFeature(row);
                        ID8List PGEList = looptest(row);

                        AddError(error);

                        return PGEList;
                    }
                    else
                    {
                        
                    }
                    // looptest(row);
                }                
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Related Unit Records rule.", ex);
            }
            return base.InternalIsValid(row);
        }

        #endregion Override for validation rule

        private ID8List looptest(IRow pRow)
        {
            Type pMMRuleClass = this.GetType(); // Type.GetTypeFromProgID("PGE.Desktop.EDER.ValidateRelatedUnitRecords");
            System.Object obj = Activator.CreateInstance(pMMRuleClass);
            if (obj is IMMValidationRule)
            {
                
                IMMValidationRule pMMValRule = (IMMValidationRule)obj;
                ID8List pInitialList = pMMValRule.IsValid(pRow);
                ID8List pPGEList = null;
                ValidationEngine.Instance.GetModifiedList(pInitialList, ref pPGEList, base._Name);
                return pPGEList;
            }
            return null;
        }
        private IRow RelatedFeature(IRow row)
        {
            IRow result = null;

            ShellInstance si = UnitHelper.CreateShellInstance(row);
            if (si != null)
            {
                IObject obj = si.Object;
                if (obj is ESRI.ArcGIS.Geodatabase.IFeature)
                {
                    result = obj as IRow;
                    //ID8ListItem listitem = new D8ListItemClass();
                    //_ErrorList.Reset();
                    //listitem = _ErrorList.Next();
                    var x = _ErrorList;

                }
            }
            return result;
        }
    }
}
