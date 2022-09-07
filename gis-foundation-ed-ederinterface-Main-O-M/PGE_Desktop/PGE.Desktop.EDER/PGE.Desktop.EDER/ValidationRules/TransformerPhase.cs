using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("9E7A2D1E-E640-4DE2-9D73-019DAE450B0A")]
    [ProgId("PGE.Desktop.EDER.TransformerPhase")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class TransformerPhase : BaseValidationRule
    {
        #region Private
        /// <summary>
        /// constant error message
        /// </summary>
        private const string _errMessage = "The transformer must be sourced by a 3 phase conductor";
        private const int _threePhaseDesignation = 7;
        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Constructor
        public TransformerPhase()
            : base("PGE Validate Transformer Phase", SchemaInfo.Electric.ClassModelNames.Transformer)
        {
        }
        #endregion Constructor

        #region Overrides for validation rule
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
            //get the transformer phase value
            int transformerPhase = obj.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert(-1);
            //cehck for 3 phase with domain description ABC
            _logger.Debug(string.Format("Transformer phase is {0}.", transformerPhase));
            if (transformerPhase != _threePhaseDesignation)
            {
                //since transformer is not 3 phase no need to execute validation rule
                return _ErrorList;
            }

            //get feature
            IFeature traceFeature = TraceFacade.GetFirstFeatureOfType(obj, esriElementType.esriETEdge);
            //check if feature is not null
            if (traceFeature == null)
            {
                _logger.Debug("Trace feature for first edge is <Null>.");
                return _ErrorList;
            }
            //get the tracefeature phase value
            int feedingPhase = traceFeature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert(-1);
            //cehck for 3 phase with domain description ABC
            _logger.Debug(string.Format("Trace feature phase value is {0}.",feedingPhase));
            if (feedingPhase != _threePhaseDesignation)
            {
                _logger.Debug(_errMessage);
                AddError(_errMessage);
            }
            return _ErrorList;
        }
        #endregion Overrides for validation rule

    }
}
