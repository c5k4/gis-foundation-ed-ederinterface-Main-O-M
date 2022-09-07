using System;
using System.Reflection;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using Telvent.Delivery.ArcFM;
using Telvent.Delivery.Framework;

namespace Telvent.PGE.ED.Desktop.ValidationRules
{
    [ComVisible(true)]
    [Guid("62D93F45-F23D-4076-A58F-A6A299F2F21F")]
    [ProgId("Telvent.PGE.ED.ValidatesLoopFeature")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateLoopFeature : BaseValidationRule
    {
        #region Private
        /// <summary>
        /// constant error message
        /// </summary>

        private const string _errMessage = "The feature participates in loop.";


        private static string[] _modelNames = new string[] { SchemaInfo.Electric.FieldModelNames.FeederInfo };

        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly Telvent.Delivery.Diagnostics.Log4NetLogger _logger = new Telvent.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion Private
        #region Constructor
        public ValidateLoopFeature()
            : base("PGE Validate Loop Feature", _modelNames)
        {
        }
        #endregion Constructor

        #region Overrides for validation rule
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


        protected override ID8List InternalIsValid(IRow row)
        {
            //cast row as Ifeature
            IFeature obj = row as IFeature;
            //cehck if casting is successful
            if (obj == null)
            {
                _logger.Debug("Row casting to Ifeature failed.");
                return _ErrorList;
            }

            //get the feederInfo value
            int feederInfo = obj.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.FeederInfo).Convert(-1);

            _logger.Debug(string.Format("{0} FeederInfo: {1}", obj.Class.AliasName, feederInfo));

            BitArray bitArray = new BitArray(BitConverter.GetBytes(feederInfo).ToArray());

            //Check the loop flag -- the 6th bit
            bool isLoop = bitArray.Get(6);

            if (isLoop)
            {
                _logger.Debug(_errMessage);
                AddError(_errMessage);
            }

            return _ErrorList;
        }

        #endregion Overrides for validation rule
    }
}
