using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

using PGE.Desktop.EDER.AutoUpdaters;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("7E3D5A90-6BEA-4F95-8735-9C7EF9470D9B")]
    [ProgId("PGE.Desktop.EDER.ValidateOperatingNumber")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateOperatingNumber : BaseValidationRule
    {
        #region Constructors
        public ValidateOperatingNumber()
            : base("PGE Validate Operating Number")
        {    
        }
        #endregion Constructors

        #region Private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string [] _modelNames = new string[]{SchemaInfo.Electric.FieldModelNames.OperatingNumber};
        
        private void AddError(List<string> _errList, IObject pObject)
        {
            if (_errList == null)
            {
                _logger.Debug("No errors found.");
                return;
            }
            foreach (string errMsg in _errList)
            {
                _logger.Debug(errMsg);
                AddError(errMsg);
            }
        }
        #endregion Private

        #region Validations overrides

        /// <summary>
        /// Returns true if "OperatingNumber" Class Model Name  and Field ModelName exist on Objectclass. 
        /// Overriden to take care of this special case where OperatingNumber class model name should be 
        /// assigned to FeatureClass as well as Field.
        /// 
        /// </summary>
        /// <param name="param">the ObjectClass</param>
        /// <returns></returns>
        protected override bool EnableByModelNames(object param)
        {
            if (!(param is IObjectClass))
            {
                _logger.Debug("Parameter is not type of IObjectClass, exiting");
                return false;
            }

            IObjectClass oclass = param as IObjectClass;
            _logger.Debug("ObjectClass:" + oclass.AliasName);
            //Check if ClassModelName exist on current ObjectClass
            bool enableForClassModel = ModelNameFacade.ContainsClassModelName(oclass, _modelNames);
            _logger.Debug("ClassModelName:" + _modelNames[0] + ", in ObjectClass :" + oclass.AliasName + "exist(" + enableForClassModel + ")");

            //Check if FieldModelName exist on current ObjectClass fields
            bool enableForFieldModel = ModelNameFacade.ContainsFieldModelName(oclass, _modelNames);
            _logger.Debug("FieldModelName:" + _modelNames[0] + ", in ObjectClass :" + oclass.AliasName + "exist(" + enableForFieldModel + ")");

            _logger.Debug(string.Format("Returning Visible:{0}", enableForClassModel && enableForFieldModel));
            return (enableForClassModel && enableForFieldModel);
        }

        /// <summary>
        /// Validtes the object for defined rule.
        /// </summary>
        /// <param name="row">the Object to be validated.</param>
        /// <returns>Error list</returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //If this rule is being filtered out - do not run it 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            // Nuisance rule modification (only works when version used, not map selection).
            // This rule should only run on inserts.
            if (!CommonValidate.ExcludeRule("", esriDifferenceType.esriDifferenceTypeInsert, row))
            {
                _logger.Debug("OperatingNumber Validation excluded based on insert only.");
                return _ErrorList;
            }

            _logger.Debug("OperatingNumber Validation started.");

            _logger.Debug("Executing EvaluationEngine.Instance.GetOpertingNumberCriteria");
            //Get all the errors depending on XML criteria
            List<string> _err_List = EvaluationEngine.Instance.GetOpertingNumberCriteria(row as IFeature);
            _logger.Debug("Executed EvaluationEngine.Instance.GetOpertingNumberCriteria");

            _logger.Debug("Executing AddError");
            //Add all the errors to the base._ErrorList 
            AddError(_err_List, (IObject)row);
            _logger.Debug("Executed AddError");
            _logger.Debug("OperatingNumber Validation end.");
            return _ErrorList;
        }
        #endregion Validations overrides
    }
}
