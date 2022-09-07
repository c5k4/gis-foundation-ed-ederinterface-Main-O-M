using System.Collections.Generic;
using System.Reflection;
using System.Xml.Schema;

namespace PGE.Desktop.EDER.SymbolNumber.Schema
{
    /// <summary>
    /// A list of errors and warnings generated during the schema validation process.
    /// </summary>
    public class SchemaErrorList
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaErrorList"/> class.
        /// </summary>
        public SchemaErrorList()
        {
            //initializing the new instance
            Errors = new List<string>();
            Warnings = new List<string>();
            Count = 0;
        }
        #endregion Constructor

        #region Public
        #region Properties
        /// <summary>
        /// Gets the errors.
        /// </summary>
        public List<string> Errors { get; private set; }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        public List<string> Warnings { get; private set; }

        /// <summary>
        /// Gets number of messages.
        /// </summary>
        public int Count { get; private set; }
        #endregion Properties

        /// <summary>
        /// Listens to XML validation error events and adds the errors to the correct list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Xml.Schema.ValidationEventArgs"/> instance containing the event data.</param>
        public void ValidationCallback(object sender, ValidationEventArgs args)
        {
            //check if error occurred is of Severity = Error 
            if (args.Severity == XmlSeverityType.Error)
            {
                //add error
                _logger.Debug(string.Format("Argument is of type Error with msg: {0}", args.Message));
                Errors.Add(args.Message);
            }

            //check if error occurred is of Severity = Warning
            if (args.Severity == XmlSeverityType.Warning)
            {
                //add warning
                _logger.Debug(string.Format("Argument is of type Warning with msg: {0}", args.Message));
                Warnings.Add(args.Message);
            }

            //increase the count
            Count++;
            _logger.Debug(string.Format("Count after increment is {0}", Count));
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
