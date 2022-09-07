using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    public class Count
    {
        #region Private
        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Constructor

        /// <summary>
        /// initialize the instance with default properties value
        /// </summary>
        public Count()
        {
            Condition = true;
        }
        #endregion Constructor

        #region Public
        #region Properties
        /// <summary>
        /// A list of valid or invalid strings depending on the value of condition.
        /// </summary>
        [XmlElement(typeof(int), ElementName = "Value")]
        public List<int> Values;

        /// <summary>
        /// Gets or sets the condition value default true
        /// </summary>
        [XmlAttribute("Condition")]
        public bool Condition { get; set; }
        #endregion Properties

        /// <summary>
        /// Initialize the childsif any.
        /// </summary>
        /// <param name="objectClass"></param>
        public void Initialize(ESRI.ArcGIS.Geodatabase.IObjectClass objectClass)
        {
            //since no childs are there no need to execute anything.
        }

        /// <summary>
        /// evaluate the condition and return the result.
        /// </summary>
        /// <param name="featureCount">The feature count.</param>
        /// <returns></returns>
        public bool Evaluate(int featureCount)
        {
            //depend on = or <> check for condition
            if (Condition)
            {
                //return if values do have this count
                _logger.Debug(string.Format("Condition is true checking if rule values do have feature count {0}.", featureCount));
                return Values.Contains(featureCount);
            }
            else
            {
                //return if values do not have this count
                _logger.Debug(string.Format("Condition is false checking if rule values do not have feature count {0}.", featureCount));
                return !Values.Contains(featureCount);
            }
        }
        #endregion Public
    }
}
