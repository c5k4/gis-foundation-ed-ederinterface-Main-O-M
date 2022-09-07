using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    /// <summary>
    /// A FeatureAttribute matches a set of values to the provided object.  The value list for a 
    /// FeatureAttribute will be interpreted as an IN if the condition value is set to true.
    /// If the condition is set to false the evaluation will return true is there are no matches 
    /// in the attribute list (NOT IN).
    /// </summary>
    public class FeatureAttribute
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureAttribute"/> class.
        /// </summary>
        public FeatureAttribute()
        {
            Condition = true;
        }
        #endregion Constructor

        #region Private
        #region Properties
        /// <summary>
        /// Keep the field index to prevent multiple calls 
        /// </summary>
        [XmlIgnore]
        private int fieldIx = -1;
        #endregion Properties

        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Public
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FeatureAttribute"/> should evaluate normally or as a NOT.
        /// </summary>
        /// <value>
        ///   <c>true</c> if condition; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("Condition")]
        public bool Condition { get; set; }

        /// <summary>
        /// Gets or sets the model name of the feature attribute which will be compared to the Value list.
        /// </summary>
        /// <value>
        /// The name of the model.
        /// </value>
        [XmlAttribute("ModelName")]
        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets the field name of the feature attribute which will be compared to the Value list.
        /// </summary>
        /// <value>
        /// The name of the model.
        /// </value>
        [XmlAttribute("FieldName")]
        public string FieldName { get; set; }

        /// <summary>
        /// A list of valid or invalid strings depending on the valuer of condition.
        /// </summary>
        [XmlElement(typeof(string), ElementName = "Value")]
        public List<string> Values;

        #endregion Properties

        /// <summary>
        /// Initializes the specified object class by determining the field index of the assigned model name.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(IObjectClass objectClass)
        {
            //check is model name is not null
            if (ModelName != null)
            {
                //retrieve the field index from field model name
                fieldIx = ModelNameFacade.FieldIndexFromModelName(objectClass, ModelName);
                _logger.Debug(string.Format("Field index {0} for field modelname {1}", fieldIx, ModelName));
            }
            else
            {
                //retrieve the field index from field name
                fieldIx = objectClass.FindField(FieldName);
                _logger.Debug(string.Format("Field index {0} for field name {1}", fieldIx, FieldName));
            }
        }

        /// <summary>
        /// Adds model names to a list for validation purposes.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void AppendModelNames(List<string> modelNames)
        {
            //check if model name is not null and doesnt contain in list of modelnames add it
            if (ModelName != null && !modelNames.Contains(ModelName))
            {
                _logger.Debug(string.Format("Adding the modelname {0} in list of modelnames.", ModelName));
                modelNames.Add(ModelName);
            }
        }


        /// <summary>
        /// Evaluates the specified feature.
        /// If condition is true return true if the value of the field specified by the assigned 
        /// field model name is contained
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public bool Evaluate(IObject feature)
        {
            //depending on = or <> check condition value 
            if (Condition)
            {
                //check if value contains list of valid values
                if (Values.Contains(getFeatureValue(feature)))
                {
                    _logger.Debug("Value contains in list of values, returning true.");
                    return true;
                }
            }
            else
            {
                //check if value doesnt contains in list of invalid values
                if (!Values.Contains(getFeatureValue(feature)))
                {
                    _logger.Debug("Value doen't contains in list of values, returning true.");
                    return true;
                }
            }

            _logger.Debug("Value doen't contains in list of values.");
            return false;
        }

        /// <summary>
        /// Gets the value of the field specified in the value tag.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public string getFeatureValue(IObject feature)
        {
            //get the feature value and convert it to string
            object value = feature.get_Value(fieldIx);
            _logger.Debug(string.Format("Value is {0}.", value));
            //convert the value to string 
            return StringFacade.GetDefaultNullString(value, "");

        }
        #endregion Public

    }
}
