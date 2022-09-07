using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Xml.Serialization;

namespace Telvent.PGE.ED.Desktop.AutoUpdaters.Special.SymbolNumber.Schema
{
    /// <summary>
    /// The criteria object represents the top definition for a symbol number criteria.  
    /// Criteria must contain one logical operator or one set of attribute values.
    /// </summary>
    public class Criteria : IEvaluationCriteria
    {
        /// <summary>
        /// Gets or sets the logical operator.  A criteria can contain one logical operator or one attribute.
        /// </summary>
        /// <value>
        /// The logical operator.
        /// </value>
        [XmlElement(typeof(LogicalOperator))]
        public LogicalOperator LogicalOperator { get; set; }

        /// <summary>
        /// Gets or sets the feature attribute.
        /// </summary>
        /// <value>
        /// The feature attribute.
        /// </value>
        [XmlElement(typeof(FeatureAttribute), ElementName="Attribute")]
        public FeatureAttribute FeatureAttribute { get; set; }

        /// <summary>
        /// Gets or sets the symbol number which will be returned by the evaluate function if all logical operators
        /// or attribute return true when evaluated.
        /// </summary>
        /// <value>
        /// The symbol number.
        /// </value>
        [XmlElement(typeof(int))]
        public int SymbolNumber { get; set; }

        /// <summary>
        /// Evaluates the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public bool Evaluate(IFeature feature) {

            if (LogicalOperator != null)
            {
                return LogicalOperator.Evaluate(feature);
            }

            if (FeatureAttribute != null)
            {
                return FeatureAttribute.Evaluate(feature);
            }

            return false;
        }

        /// <summary>
        /// Initializes child objects using the provided object class.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(IObjectClass objectClass)
        {
            if (LogicalOperator != null) {
                LogicalOperator.Initialize(objectClass);
            }

            if (FeatureAttribute != null) {
                FeatureAttribute.Initialize(objectClass);
            }
        }

        public void AppendModelNames(List<string> modelNames){

            if (LogicalOperator != null) {
                LogicalOperator.AppendModelNames(modelNames);
            }

            if (FeatureAttribute != null) {
                FeatureAttribute.AppendModelNames(modelNames);
            }
        }
    }
}
