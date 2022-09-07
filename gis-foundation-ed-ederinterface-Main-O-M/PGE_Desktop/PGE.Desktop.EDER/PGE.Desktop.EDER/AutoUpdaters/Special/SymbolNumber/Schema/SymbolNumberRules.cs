using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    /// <summary>
    /// A class representing a feature class and the rules used to determine it's symbol number.
    /// </summary>
    public class SymbolNumberRules
    {
        #region Public
        // initialize serializer. XmlSerializer is thread safe, only a single instance is necessary.
        public static System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SymbolNumberRules));

        /// <summary>
        /// The feature class name.  Used for indexing.
        /// </summary>
        [XmlElement(typeof(FeatureClass), ElementName = "FeatureClass")]
        public FeatureClass featureClass;

        /// <summary>
        /// Deserializes the specified stream.  Caller must handle exceptions thrown by the XmlSerializer.Deseralize method.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static SymbolNumberRules Deserialize(Stream stream)
        {
            try
            {
                //return XML deserialized object
                _logger.Debug("Returning the deserialized stream.");
                return serializer.Deserialize(stream) as SymbolNumberRules;
            }
            catch (Exception ex)
            {
                //log the exception and throw it as well
                _logger.Warn("Error occurred while deserializing the stream.", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the symbol number bye evaluating the rules defined in the XML stream.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public int GetSymbolNumber(IFeature feature)
        {
            //return the symbol number from feature class object
            _logger.Debug("Returning symbol number from feature class object.");
            return featureClass.GetSymbolNumber(feature);
        }

        /// <summary>
        /// Gets List of errors validating operating number per rule defined in the XML stream.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public List<string> ValidateOperatingNumber(IFeature feature)
        {
            //return the Operating number validations from feature class object
            _logger.Debug("Returning the validated operating number from feature class object.");
            return featureClass.ValidateOperatingNumber(feature);
        }

        /// <summary>
        /// Initializes the specified object class by passing initialization parameters to children.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(IObjectClass objectClass)
        {
            //initializing the feature class object for objectclass
            _logger.Debug("Initializing the feature class object from ObjectClass:" + objectClass.AliasName);
            featureClass.Initialize(objectClass);
            _logger.Debug("Successfully initialized the feature class object from ObjectClass:" + objectClass.AliasName);
        }

        /// <summary>
        /// Gets the a list of model names used in child elements.
        /// </summary>
        /// <returns></returns>
        public List<string> GetModelNames()
        {
            List<string> modelNames = new List<string>();
            //retrieve Modelnames from feature class object
            _logger.Debug("Retriving the model names from feature class object.");
            featureClass.AppendModelNames(modelNames);
            _logger.Debug(string.Format("{0} Model names retrieved", modelNames == null ? 0 : modelNames.Count));
            return modelNames;
        }

        /// <summary>
        /// Validates the XML stream against the schema definition, and returns a list of errors and warnings.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static SchemaErrorList ValidateXML(Stream stream)
        {
            // Initialize the error list and set up XML IO classes.
            SchemaErrorList errorList = new SchemaErrorList();

            XmlReaderSettings settings = null;
            XmlReader reader = null;

            try
            {
                // Validate the XML stream against the schema definition
                settings = new XmlReaderSettings();
                settings.Schemas.Add(getSchema());
                settings.ValidationEventHandler += new ValidationEventHandler(errorList.ValidationCallback);
                settings.ValidationType = ValidationType.Schema;

                //Create the schema validating reader.
                reader = XmlReader.Create(stream, settings);

                // Execute the reader which triggers the validation process.
                while (reader.Read()) ;

            }
            catch
            {
                // Errors are logged by errorList.ValidationCallback
                //add error
                _logger.Debug(string.Format("Invalid schema found."));
                errorList.Errors.Add("Invalid schema found.");
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            // Return errors.
            return errorList;

        }
        #endregion Public

        #region Private
        /// <summary>
        /// logger to log all the info, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// XMLSchema for symbol number rules.
        /// </summary>
        [XmlIgnore]
        private static XmlSchema _schema;

        /// <summary>
        /// Initializes the _schema object by loading the embedded xsd file.
        /// </summary>
        /// <returns></returns>
        private static XmlSchema getSchema()
        {

            // Initialize the schema object if necissary.
            if (_schema == null)
            {
                Stream stream = null;
                try
                {
                    // Load schema from resource stream.
                    stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.AutoUpdaters.Special.SymbolNumber.Schema.SymbolNumber.xsd");
                    _logger.Debug("Loading schema from the 'PGE.Desktop.EDER.AutoUpdaters.Special.SymbolNumber.Schema.SymbolNumber.xsd' stream.");
                    _schema = XmlSchema.Read(stream, null);
                    _logger.Debug("Schema loaded successfully.");
                }
                finally
                {
                    _logger.Debug("closing the stream if it's not <Null>.");
                    // Cleanup 
                    if (stream != null)
                    {
                        _logger.Debug("closing the stream object.");
                        stream.Close();
                        _logger.Debug("closed the stream object.");
                    }
                }
            }
            _logger.Debug("Returning schema object");
            return _schema;
        }
        #endregion Private
    }
}
