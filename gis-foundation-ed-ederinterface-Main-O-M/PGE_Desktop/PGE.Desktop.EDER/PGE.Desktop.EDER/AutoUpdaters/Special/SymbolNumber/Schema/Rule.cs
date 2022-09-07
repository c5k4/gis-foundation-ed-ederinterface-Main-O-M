using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

using ESRI.ArcGIS.Geodatabase;

using PGE.Desktop.EDER.AutoUpdaters;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    /// <summary>
    /// The criteria object represents the top definition for a symbol number criteria.  
    /// Criteria must contain one logical operator or one set of attribute values.
    /// </summary>
    public class Rule : IEvaluationCriteria
    {
        #region Public
        #region Properties
        /// <summary>
        /// Gets or sets the logical operator.  A criteria can contain one logical operator or one attribute.
        /// </summary>
        /// <value>
        /// The logical operator.
        /// </value>
        [XmlElement(typeof(LogicalOperator), ElementName = "LogicalOperator")]
        [XmlElement(typeof(AndOperator), ElementName = "And")]
        [XmlElement(typeof(OrOperator), ElementName = "Or")]
        public LogicalOperator LogicalOperator { get; set; }

        [XmlElement(typeof(Relationship), ElementName = "Relationship")]
        public Relationship Relationship { get; set; }

        /// <summary>
        /// Gets or sets the feature attribute.
        /// </summary>
        /// <value>
        /// The feature attribute.
        /// </value>
        [XmlElement(typeof(FeatureAttribute), ElementName = "Attribute")]
        public FeatureAttribute FeatureAttribute { get; set; }

        /// <summary>
        /// Gets or sets the symbol number which will be returned by the evaluate function if all logical operators
        /// or attribute return true when evaluated.
        /// </summary>
        /// <value>
        /// The symbol number.
        /// </value>
        [XmlAttribute("SymbolNumber")]
        public int SymbolNumber { get; set; }

        /// <summary>
        /// Gets or sets the Valid begins with character for operating number.
        /// </summary>
        [XmlAttribute("BeginsWith")]
        public string BeginsWith { get; set; }

        /// <summary>
        /// Gets or sets valid ends with number 0=Even/ 1=Odd/ 2= any number for operating number.
        /// </summary>
        [XmlAttribute("EndsWith")]
        public int EndsWith { get; set; }

        /// <summary>
        /// Gets or sets Minimum required length of operating number.
        /// </summary>
        [XmlAttribute("MinLength")]
        public int MinLength { get; set; }

        /// <summary>
        /// Gets or sets valid Maximum length of operating number.
        /// </summary>
        [XmlAttribute("MaxLength")]
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets Required or not 0=Optional / 1=Required for operating number.
        /// </summary>
        [XmlAttribute("Required")]
        public int Required { get; set; }
        #endregion Properties

        /// <summary>
        /// Evaluates the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public bool Evaluate(IObject feature)
        {
            //check if logical operator is not null
            if (LogicalOperator != null)
            {
                //return logical operator.evaluate
                _logger.Debug("Executing LogicalOperator.Evaluate.");
                return LogicalOperator.Evaluate(feature);
            }

            //check if relationship operator is not null
            if (Relationship != null)
            {
                //return relationship operator.evaluate
                _logger.Debug("Executing Relationship.Evaluate.");
                return Relationship.Evaluate(feature);
            }

            //check if feature attribute is not null
            if (FeatureAttribute != null)
            {
                //return feature attribute.evaluate
                _logger.Debug("Executing FeatureAttribute.Evaluate.");
                return FeatureAttribute.Evaluate(feature);
            }

            //if everything is null then return null
            _logger.Debug("Nothing to Evaluate returning <Null>.");
            return false;
        }

        /// <summary>
        /// Initializes child objects using the provided object class.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(IObjectClass objectClass)
        {
            //check if logical operator is not null
            if (LogicalOperator != null)
            {
                //initialize the logical operator for objectClass
                _logger.Debug("Initializing LogicalOperator for ObjectClass:" + objectClass.AliasName);
                LogicalOperator.Initialize(objectClass);
                _logger.Debug("Initialization completed for ObjectClass:" + objectClass.AliasName);
            }
            //check if feature attribute is not null
            if (FeatureAttribute != null)
            {
                //initialize the FeatureAttribute for objectClass
                _logger.Debug("Initializing FeatureAttribute for ObjectClass:" + objectClass.AliasName);
                FeatureAttribute.Initialize(objectClass);
                _logger.Debug("Initialization completed for ObjectClass:" + objectClass.AliasName);
            }
        }

        /// <summary>
        /// Used to create a list of model names used in the configuration document.
        /// </summary>
        /// <param name="modelNames">The model names.</param>
        public void AppendModelNames(List<string> modelNames)
        {
            //check if logical operator is not null
            if (LogicalOperator != null)
            {
                //append the modelnames to logical operator
                _logger.Debug("Appending the model names to logical operator.");
                LogicalOperator.AppendModelNames(modelNames);
                _logger.Debug("Completed appending the model names to logical operator.");
            }
            //check if feature attribute is not null
            if (FeatureAttribute != null)
            {
                //append the modelnames to feature attribute
                _logger.Debug("Appending the model names to feature attribute.");
                FeatureAttribute.AppendModelNames(modelNames);
                _logger.Debug("Completed appending the model names to feature attribute.");
            }
        }
        #endregion Public

        #region Private
        /// <summary>
        /// logger to log all the inforation, warning and error
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private
    }
}
