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
    [Guid("CD4FEE55-C2CC-4592-8A2C-D74848B4F3FD")]
    [ProgId("PGE.Desktop.EDER.ValidateIslandFeature")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateIslandFeature : BaseValidationRule{
        #region Private
        /// <summary>
        /// constant error message
        /// </summary>
        private const string _errMessage = "Island Feature. The feature is not connected to any feeder.";

        private static string[] _modelNames = new string[] { SchemaInfo.Electric.FieldModelNames.FeederInfo }; //SchemaInfo.Electric.FieldModelNames.FeederInfo

        private const int _inServiceStatus = 5;

        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Constructor
        public ValidateIslandFeature()
            : base("PGE Validate Island Feature", _modelNames) //SchemaInfo.Electric.ClassModelNames ??
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
            //Check if ClassModelName exist on current ObjectClass
            //bool enableForClassModel = ModelNameFacade.ContainsClassModelName(oclass, _modelNames);
            //_logger.Debug("ClassModelName:" + _modelNames[0] + ", in ObjectClass :" + oclass.AliasName + "exist(" + enableForClassModel + ")");

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

            if (currentStatus == _inServiceStatus)
            {
                //get the feederInfo value
                string[] circuitIDs = FeederManager2.GetCircuitIDs(feature);
               //If there are no circuit IDs assigned then this is an island feature.
                bool isIsland = circuitIDs.Length < 1;

                if (isIsland)
                {
                    _logger.Debug(_errMessage);
                    AddError(_errMessage);
                }
            }
            else
            {
                _logger.Debug("A status is other than InService no need to validate.");
            }

            return _ErrorList;
        }
        #endregion Overrides for validation rule
    }
}
