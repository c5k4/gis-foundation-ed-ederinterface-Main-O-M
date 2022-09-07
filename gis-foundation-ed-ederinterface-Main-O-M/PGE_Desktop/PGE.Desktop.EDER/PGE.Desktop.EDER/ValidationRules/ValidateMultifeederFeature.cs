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
    [Guid("87B57002-125D-48FB-BE70-A9D8EAE0AD8C")]
    [ProgId("PGE.Desktop.EDER.ValidatesMultifeederFeature")]
    [ComponentCategory(ComCategory.MMValidationRules)]

  public   class ValidateMultifeederFeature:BaseValidationRule 
    {
        #region Private
        /// <summary>
        /// constant error message
        /// </summary>

        private const string _errMessage = "Feature is connected to multiple feeders.";

        private static string[] _modelNames = new string[] { SchemaInfo.Electric.FieldModelNames.FeederInfo }; //SchemaInfo.Electric.FieldModelNames.FeederInfo

        private const int _inServiceStatus = 5;

        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private
        #region Constructor
        public ValidateMultifeederFeature()
            : base("PGE Validate Multi-feed Feature", _modelNames) //SchemaInfo.Electric.ClassModelNames ??
        {
        }
        #endregion Constructor
        /// <summary>
        /// Returns true if "FeederInfo" Field ModelName exist on Objectclass. 
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

        #region Overrides for validation rule
        protected override ID8List InternalIsValid(IRow row)
        {
            //cast row as Ifeature
            IFeature feature = row as IFeature;
            //cehck if casting is successful
            if (feature == null)
            {
                _logger.Debug("Row casting to Ifeature failed.");
                return _ErrorList;
            }

            int currentStatus = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).Convert(-1);
            //Check if status is the default value
            if (currentStatus == -1)
            {
                _logger.Debug("Status is <Null>.");
                return _ErrorList;
            }

            IField feederManagerNonTraceable = ModelNameFacade.FieldFromModelName(feature.Class, SchemaInfo.Electric.FieldModelNames.FeederManagerNonTraceable);
            bool shouldValidate = false;

            if (currentStatus == _inServiceStatus)
            {
                if (feederManagerNonTraceable != null)
                {
                    object nonTraceableValue = feature.get_Value(feature.Fields.FindField(feederManagerNonTraceable.Name));
                    if (nonTraceableValue != null && nonTraceableValue.ToString() != "1")
                    {
                        //Non traceable field is set to traceable so we should validate this feature
                        shouldValidate = true;
                    }
                }
                else
                {
                    shouldValidate = true;
                }
            }
            else
            {
                _logger.Debug("As status is other than InService no need to validate.");
            }

            if (shouldValidate)
            {
                //Determine if the feature is a tie device
                bool isTieDevice = FeederManager2.IsTieDevice(feature);

                if (!isTieDevice)
                {
                    //Determine if the feature is multifed
                    bool isMultiFed = FeederManager2.IsMultiFed(feature);

                    if (isMultiFed)
                    {
                        _logger.Debug(_errMessage);
                        AddError(_errMessage);
                    }
                }
                else
                {
                    _logger.Debug("Tie Device -- not validating multi-feed flag.");
                }
            }

            return _ErrorList;
        }
        #endregion Overrides for validation rule
    }
}
