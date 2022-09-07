using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;

using Miner.Interop;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComponentCategory(ComCategory.MMValidationRules)]
    [ComVisible(true)]
    [Guid("04B41DC9-4122-427c-9E58-93A759219C5D")]
    public class ValidateESRIRules : BaseValidationRule
    {
        #region Private fields

        private string _progIdOfRule = "MMStandardRules.MMEsriRules";
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string[] _modelNames = new string[] { SchemaInfo.Electric.FieldModelNames.OperatingNumber }; 

        #endregion

        #region Constructor
        public ValidateESRIRules()
            : base("PGE Validate ESRI Rules") 
        {

        }
        #endregion
        
        #region IMMValidationRule Members

        protected override ID8List InternalIsValid(IRow pRow)
        {
            try
            {
                //Call the COM object IsValid passing in the row
                Type pMMRuleClass = Type.GetTypeFromProgID(_progIdOfRule);
                System.Object obj = Activator.CreateInstance(pMMRuleClass);
                if (obj is IMMValidationRule)
                {
                    IMMValidationRule pMMValRule = (IMMValidationRule)obj;
                    ID8List pInitialList = pMMValRule.IsValid(pRow);
                    ID8List pPGEList = null;
                    ValidationEngine.Instance.GetModifiedList(pInitialList, ref pPGEList, base._Name);
                    return pPGEList;
                }
                else
                    _logger.Debug("Error Rule is not IMMValidationRule");

                return _ErrorList;
            }
            catch (Exception ex)
            {
                _logger.Debug("Error in rule: " + base.Name + " " + ex.Message);
                throw;
            }

        }

        #endregion

    }
}
 


