using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace PGE.Common.Delivery.Systems.Xml
{
    /// <summary>
    /// A supporting class used to validate an XML file against either a document type definition
    /// (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
    /// </summary>
    public sealed class XmlValidation
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlValidation"/> class.
        /// </summary>
        /// <param name="schemaUri">The schema URI which can be either a document type definition
        /// (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.</param>
        /// <param name="validationType">Type of the validation.</param>
        public XmlValidation(string schemaUri, ValidationType validationType)
            : this(schemaUri, null, validationType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlValidation"/> class.
        /// </summary>
        /// <param name="schemaUri">The schema URI which can be either a document type definition
        /// (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.</param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <param name="validationType">Type of the validation.</param>
        public XmlValidation(string schemaUri, string targetNamespace, ValidationType validationType)
        {
            this.SchemaUri = schemaUri;
            this.TargetNamespace = targetNamespace;
            this.ValidationType = validationType;
        }

        #endregion

        #region Events

        /// <summary>
        /// The event handler for receiving information about document type definition
        /// (DTD), XML-Data Reduced (XDR) schema, and XML Schema definition language
        /// (XSD) schema validation errors.
        /// </summary>
        public event ValidationEventHandler Errors;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the schema URI.
        /// </summary>
        /// <value>The schema URI.</value>
        public string SchemaUri
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the target namespace.
        /// </summary>
        /// <value>The target namespace.</value>
        public string TargetNamespace
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the validation.
        /// </summary>
        /// <value>The type of the validation.</value>
        public ValidationType ValidationType
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates the specified document with either a document type definition
        /// (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </summary>
        /// <param name="document">The document.</param>
        public void Validate(XmlDocument document)
        {
            this.Validate(document.OuterXml, new XmlReaderSettings());
        }

        /// <summary>
        /// Validates the specified XML fragment with either a document type definition
        /// (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </summary>
        /// <param name="xmlFragment">The XML fragment.</param>
        public void Validate(string xmlFragment)
        {
            this.Validate(xmlFragment, new XmlReaderSettings());
        }

        /// <summary>
        /// Validates the specified XML fragment with either a document type definition
        /// (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </summary>
        /// <param name="xmlFragment">The XML fragment.</param>
        /// <param name="settings">The settings.</param>
        public void Validate(string xmlFragment, XmlReaderSettings settings)
        {
            // Read the XSD into the schema set.
            XmlSchemaSet xss = new XmlSchemaSet();
            xss.Add(this.TargetNamespace, this.SchemaUri);

            // Set the validation settings.
            settings.Schemas.Add(xss);
            settings.ValidationType = this.ValidationType;
            settings.ValidationEventHandler += this.Errors;

            // Load the Xml fragment into the stream.
            using (StringReader sr = new StringReader(xmlFragment))
            {
                // Load the XML reader and perform the validation.
                XmlTextReader xtr = new XmlTextReader(sr);
                XmlReader xr = XmlReader.Create(xtr, settings);

                // Read each line which will compare it against the XSD schema.
                while (xr.Read()) ;
            }
        }

        #endregion
    }
}