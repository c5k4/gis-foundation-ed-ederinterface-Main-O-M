using System;
using System.Reflection;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Framework.FeederManager;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("1AED5E1A-BF55-4DD0-B8D0-91CF1796BEF9")]
    [ProgId("PGE.Desktop.EDER.ValidatesMultifeederFeatureNonSecondary")]
    [ComponentCategory(ComCategory.MMValidationRules)]

public class ValidateMultifeederFeatureNonSecondary:BaseValidationRule
 {
        #region Private
        /// <summary>
        /// constant error message
        /// </summary>

        private const string _errMessage = "Feature is connected to multiple feeders.";

        private static string[] _modelNames = new string[] { SchemaInfo.Electric.FieldModelNames.FeederInfo, SchemaInfo.Electric.FieldModelNames.SecondaryIDC }; //SchemaInfo.Electric.FieldModelNames.FeederInfo and SchemaInfo.Electric.FieldModelNames.SecondaryIDC

        private const int _inServiceStatus = 5;

        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private
        #region Constructor
        public ValidateMultifeederFeatureNonSecondary()
            : base("PGE Validate Multi-feeder Non-Secondary Feature", _modelNames) //SchemaInfo.Electric.ClassModelNames ??
        {
        }
        #endregion Constructor
        /// <summary>
        /// Returns true if "FeederInfo" of "SecondaryIDC" Field ModelName exist on Objectclass. 
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
          
            //Check if FieldModelName exist on current ObjectClass fields
            bool enableForFieldModel = ModelNameFacade.ContainsFieldModelName(oclass, _modelNames);
            _logger.Debug("FieldModelName:" + _modelNames[0] + ", in ObjectClass :" + oclass.AliasName + "exist(" + enableForFieldModel + ")");

            _logger.Debug(string.Format("Returning Visible:{0}", enableForFieldModel));

            return (enableForFieldModel);
    }

        protected override ID8List InternalIsValid(IRow row)
        {
            bool runQAQC=true;
            //cast row as Ifeature
            IFeature feature = row as IFeature;
            //cehck if casting is successful
            if (feature == null)
            {
                _logger.Debug("Row casting to Ifeature failed.");
                return _ErrorList;
            }

            string isSecondary = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.SecondaryIDC).ToString();
            //Check if status is the default value
            if (isSecondary != null)
            {
                if (isSecondary =="Y")
                {
                  _logger.Debug("SecondaryIDC is set, so bypassing QA/QC");
                  runQAQC = false;
                }                
            }

            if (runQAQC)
            {
                PGE.Desktop.EDER.ValidationRules.ValidateMultifeederFeature vr = new ValidateMultifeederFeature();
                _ErrorList = vr.IsValid(row);
            }
            return _ErrorList;
        }
 }
}

