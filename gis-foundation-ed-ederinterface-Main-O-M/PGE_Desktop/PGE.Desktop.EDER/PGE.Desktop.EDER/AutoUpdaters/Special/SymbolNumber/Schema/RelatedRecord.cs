using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    public class RelatedObject
    {
        #region Public
        #region Properties
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
        /// Gets or sets the Relationships of the feature.
        /// </summary>
        /// <value>
        /// Relationships of the feature.
        /// </value>
        [XmlElement(typeof(Relationship), ElementName = "Relationship")]
        public List<Relationship> Relationships { get; set; }
        #endregion Properties

        /// <summary>
        /// Initializes the related this object with object class.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(ESRI.ArcGIS.Geodatabase.IObjectClass objectClass)
        {
            //iterate through all attributes for individual attibute initialization with object class
            _logger.Debug("Iterating through all the attribute to initialize for object class:" + objectClass.AliasName);
            foreach (FeatureAttribute attribute in FeatureAttributes)
            {
                //initialize attribute for object class
                _logger.Debug(string.Format("Initializing attribute for field name '{0}' or model name '{1}' with object class:{2}", attribute.FieldName, attribute.ModelName, objectClass.AliasName));
                attribute.Initialize(objectClass);
                _logger.Debug(string.Format("Completed Initialization of attribute for field name '{0}' or model name '{1}' with object class:{2}", attribute.FieldName, attribute.ModelName, objectClass.AliasName));
            }

            //initialize relationships for object class
            foreach (Relationship rel in Relationships)
            {
                _logger.Debug(string.Format("Initializing relationship '{0}' for Object class:{1}", rel.ClassName, objectClass.AliasName));
                rel.Initialize(objectClass);
                _logger.Debug(string.Format("Initializing relationship '{0}' for Object class:{1}", rel.ClassName, objectClass.AliasName));
            }

        }

        /// <summary>
        /// Evaluates this object for object.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public bool Evaluate(IObject row)
        {
            //iterate for each attribute
            _logger.Debug("Iterating through all the attributes for row.");

            foreach (FeatureAttribute attribute in FeatureAttributes)
            {
                //if attribute evaluation is return false
                _logger.Debug("Evaluating attribute for row.");
                if (!attribute.Evaluate(row))
                {
                    //return false
                    _logger.Debug(string.Format("Evaluation returned false for attribute with field name '{0}' or model name '{1}'.",
                                                attribute.FieldName,
                                                attribute.ModelName));
                    return false;
                }
            }
            //if all attribute evaluations are pass then proceed
            _logger.Debug("All attribute evaluations returned true, then proceeding furthur.");

            _logger.Debug("Iterating through all the Relationships for row.");
            foreach (Relationship rel in Relationships)
            {
                if (!rel.Evaluate(row))
                {
                    _logger.Debug(string.Format("Evaluation returned false for Relationship with name '{0}' and record OID = {1}", rel.ClassName, row.OID));
                    return false;
                }
            }
            _logger.Debug("Evalutaion for all the Relationships returned true. Hence returning true.");
            return true;
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
