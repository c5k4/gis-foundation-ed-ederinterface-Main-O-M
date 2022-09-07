using System.Reflection;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    /// <summary>
    /// Serializes and deserialized an AND element in the symbol number config.
    /// </summary>
    public class AndOperator : LogicalOperator
    {
        #region Public
        /// <summary>
        /// Evaluates the specified feature.  Returns true if all child elements are true.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public override bool Evaluate(IObject feature)
        {

            //evaluate OR from base for current feature.
            _logger.Debug("Evaluating base.evaluateAND for feature.");
            return base.evaluateAND(feature);
        }
        #endregion Public

        #region Private
        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private
    }
}
