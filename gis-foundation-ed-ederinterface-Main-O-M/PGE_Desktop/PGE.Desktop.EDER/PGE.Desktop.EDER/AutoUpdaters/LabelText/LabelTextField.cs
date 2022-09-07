namespace Telvent.PGE.ED.Desktop.AutoUpdaters.LabelText
{
    /// <summary>
    ///   An class that is used directly with the <see cref="BaseLabelTextAU" /> for controlling how the label text is built.
    /// </summary>
    public sealed class LabelTextField
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LabelTextField" /> class.
        /// </summary>
        /// <param name="fieldName">The name of the field to get the value from.</param>
        public LabelTextField(string fieldName)
            : this("", fieldName, "")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LabelTextField" /> class.
        /// </summary>
        /// <param name="prefix">A string which will be inserted before the field's value.</param>
        /// <param name="fieldName">The name of the field to get the value from.</param>
        /// <param name="postfix">A string which will be appended after the field's value</param>
        /// <param name="useDomain"><c>true</c> if the field's domain should be used; otherwise false.</param>
        public LabelTextField(string prefix, string fieldName, string postfix, bool useDomain = true)
        {
            Prefix = prefix;
            FieldName = fieldName;
            Postfix = postfix;
            UseDomain = useDomain;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name of the field the value will be retrieved from.
        /// </summary>
        /// <value>The name of the field the value will be retrieved from.</value>
        public string FieldName { get; private set; }

        /// <summary>
        ///   Gets the string that will be appended to the end of the field value.
        /// </summary>
        /// <value> The postfix string. </value>
        public string Postfix { get; private set; }

        /// <summary>
        ///   Gets the string that will be appended to the start of the field value.
        /// </summary>
        /// <value> The prefix string. </value>
        public string Prefix { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether the field domain will be used to get the description.
        /// </summary>
        /// <value> <c>true</c> if domain will be used; otherwise, <c>false</c> . </value>
        public bool UseDomain { get; private set; }

        #endregion
    }
}