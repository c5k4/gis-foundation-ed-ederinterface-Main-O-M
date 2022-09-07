using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    /// <summary>
    /// LogicalOperator handles XML serialization and allows the user to evaluate criteria using
    /// AND and OR logic.  Logical operators can be nested or contain feature attributes.
    /// </summary>
    public class LogicalOperator
    {
        #region Private
        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Public
        #region Properties
        /// <summary>
        /// Gets or sets the type of the op.  Valid values are AND and OR
        /// </summary>
        /// <value>
        /// The type of the op.
        /// </value>
        [XmlAttribute("OpType")]
        public string OpType { get; set; }

        /// <summary>
        /// Gets or sets the logical operators.  All child operators are evaluated using as the
        /// operated using the assign operator type.
        /// </summary>
        /// <value>
        /// The logical operators.
        /// </value>
        [XmlElement(typeof(LogicalOperator), ElementName = "LogicalOperator")]
        [XmlElement(typeof(AndOperator), ElementName = "And")]
        [XmlElement(typeof(OrOperator), ElementName = "Or")]
        public List<LogicalOperator> LogicalOperators { get; set; }

        /// <summary>
        /// Gets or sets the feature attributes. Each feature attribute is defined using the 
        /// operator type assigned above.
        /// </summary>
        /// <value>
        /// The feature attributes.
        /// </value>
        [XmlElement(typeof(FeatureAttribute), ElementName = "Attribute")]
        public List<FeatureAttribute> FeatureAttributes { get; set; }

        /// <summary>
        /// Gets or sets relationship
        /// </summary>
        [XmlElement(typeof(Relationship), ElementName = "Relationship")]
        public List<Relationship> Relationships { get; set; }
        #endregion Properties

        /// <summary>
        /// Evaluates the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public virtual bool Evaluate(IObject feature)
        {
            //chekc for operator type
            _logger.Debug("OpType:" + OpType.ToUpper());
            switch (OpType.ToUpper())
            {
                case "OR":
                    //execute evaluateOR
                    _logger.Debug("Executing the evaluateOR operator.");
                    return evaluateOR(feature);
                case "AND":
                    //execute evaluateAND
                    _logger.Debug("Executing the evaluateAND operator.");
                    return evaluateAND(feature);
            }
            //if no valid operator found then return false.
            _logger.Debug("No valid operator found returning false.");
            return false;
        }

        /// <summary>
        /// Initializes all child operators and all assigned feature classes.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(IObjectClass objectClass)
        {
            //iterate for all the operators in logical operator
            foreach (LogicalOperator op in LogicalOperators)
            {
                //initialize all the child logical operators
                _logger.Debug("Initializing Operator : " + op.OpType + " for Object class:" + objectClass.AliasName);
                op.Initialize(objectClass);
            }
            //iterate for all the feature attributes
            foreach (FeatureAttribute attribute in FeatureAttributes)
            {
                //initialize all the child feature attributes
                _logger.Debug(string.Format("Initializing feature attribute field name '{0}' or model name '{1}' for Object class:{2}", attribute.FieldName, attribute.ModelName, objectClass.AliasName));
                attribute.Initialize(objectClass);
            }
            //iterate for all relationships
            foreach (Relationship relationship in Relationships)
            {
                //initialize all the child relationships
                _logger.Debug(string.Format("Initializing relationship '{0}' for Object class:{1}", relationship.ClassName, objectClass.AliasName));
                relationship.Initialize(objectClass);
            }
        }

        /// <summary>
        /// Append the model names.
        /// </summary>
        /// <param name="modelNames">The model names.</param>
        public void AppendModelNames(List<string> modelNames)
        {
            //iterate for all logical operators to append model names
            foreach (LogicalOperator op in LogicalOperators)
            {
                //append the model names
                _logger.Debug(string.Format("Executing append modelnames for logical operator {0}", op.OpType));
                op.AppendModelNames(modelNames);
                _logger.Debug(string.Format("Execution completed of append modelnames for logical operator {0}", op.OpType));
            }

            //iterate for all feature attributes to append model names
            foreach (FeatureAttribute attribute in FeatureAttributes)
            {
                //append the model names
                _logger.Debug(string.Format("Executing append modelnames for attribute with field name '{0}' or model name '{1}'", attribute.FieldName, attribute.ModelName));
                attribute.AppendModelNames(modelNames);
                _logger.Debug(string.Format("Execution completed of append modelnames for attribute with field name '{0}' or model name '{1}'", attribute.FieldName, attribute.ModelName));
            }

            // if (Relationship != null) Relationship.Initialize(objectClass);
        }
        #endregion Public

        #region Protected
        /// <summary>
        /// Evaluates the child logical operators and feature attributes as an OR.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        protected bool evaluateOR(IObject feature)
        {
            //iterate for all the logical operators
            foreach (LogicalOperator op in LogicalOperators)
            {
                //atleast one evaluation should evaluate to true then return true
                if (op.Evaluate(feature))
                {
                    _logger.Debug("Logical operator evaluated to true.");
                    return true;
                }
            }

            //iterate for all feature attributed
            foreach (FeatureAttribute attribute in FeatureAttributes)
            {
                //atleast one evaluation should evaluate to true then return true
                if (attribute.Evaluate(feature))
                {
                    _logger.Debug("Feature attribute evaluated to true.");
                    return true;
                }
            }

            //iterate for all realtionships
            foreach (Relationship relationship in Relationships)
            {
                //atleast one evaluation should evaluate to true then return true
                if (relationship.Evaluate(feature))
                {
                    _logger.Debug("Relationship evaluated to true.");
                    return true;
                }
            }

            _logger.Debug("None of the evaluations evaluated to true.");
            return false;
        }

        /// <summary>
        /// Evaluates the child logical operators and feature attributes as an AND.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        protected bool evaluateAND(IObject feature)
        {

            //iterate for all the logical operators
            foreach (LogicalOperator op in LogicalOperators)
            {
                //all evaluations should evaluate to true then return true else false
                if (!op.Evaluate(feature))
                {
                    _logger.Debug("Logical operator evaluated to false.");
                    return false;
                }
            }

            //iterate for all feature attributed
            foreach (FeatureAttribute attribute in FeatureAttributes)
            {
                //all evaluations should evaluate to true then return true else false
                if (!attribute.Evaluate(feature))
                {
                    _logger.Debug("Feature attribute evaluated to false.");
                    return false;
                }
            }

            //iterate for all realtionships
            foreach (Relationship relationship in Relationships)
            {
                //all evaluations should evaluate to true then return true else false
                if (!relationship.Evaluate(feature))
                {
                    _logger.Debug("relationship evaluated to false.");
                    return false;
                }
            }

            //all evaluations are evaluated to true, return true
            _logger.Debug("All the evaluations evaluated to true.");
            return true;
        }
        #endregion Protected
    }
}
