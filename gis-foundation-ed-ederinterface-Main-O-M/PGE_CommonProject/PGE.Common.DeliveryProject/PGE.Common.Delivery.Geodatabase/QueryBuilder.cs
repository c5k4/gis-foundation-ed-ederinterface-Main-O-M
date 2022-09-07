using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using ESRI.ArcGIS.Geodatabase;

namespace PGE.Common.Delivery.Geodatabase
{
    #region Enumerations

    /// <summary>
    /// Enumeration of the arithmetic operators that perform mathematical operations on two expressions of one or more of the data types of the numeric data type category. 
    /// </summary>
    public enum ArithmeticOperator
    {
        /// <summary>
        /// +	Arithmetic operator for addition.
        /// </summary>
        Add,
        /// <summary>
        /// -	Arithmetic operator for subtraction.
        /// </summary>
        Subtract,
        /// <summary>
        /// *	Arithmetic operator for multiplication.
        /// </summary>
        Multiple,
        /// <summary>
        /// /	Arithmetic operator for division.
        /// </summary>
        Divide
    }

    /// <summary>
    /// Enumeration of the comparison operators supported in SQL statements.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// This is the equals operator for strict equality comparisons. The framework automatically
        /// uses the correct syntax for null value comparisons.
        /// </summary>
        Equals,
        /// <summary>
        /// This is the negated equals operator. The framework automatically uses the correct syntax
        /// for null value comparisons.
        /// </summary>
        NotEquals,
        /// <summary>
        /// This is the equals operator for partial equality comparisons. The parameter value
        /// should be a string to match with percent characters used as wildcards.
        /// </summary>
        Like,
        /// <summary>
        /// This is the negated equals operator for partial equality comparisons. The parameter value
        /// should be a string to match with percent characters used as wildcards.
        /// </summary>
        NotLike,
        /// <summary>
        /// This is the less than operator.
        /// </summary>
        LessThan,
        /// <summary>
        /// This is the less than or equals operator.
        /// </summary>
        LessThanOrEquals,
        /// <summary>
        /// This is the greater than operator.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// This is the greater than or equals operator.
        /// </summary>
        GreaterThanOrEquals,
        /// <summary>
        /// This is the in operator which tests for set membership. Constraints using Operator.In can
        /// only be added by specifying a list of elements (though the lists may contain 0 or 1 elements).
        /// </summary>
        In,
        /// <summary>
        /// This is the negated in operator which tests for set non-membership.
        /// </summary>
        NotIn
    }

    /// <summary>
    /// Enumeration of the logic operators supported in SQL statements.
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        /// This is the AND operator. Combines two conditions together. Selects a record if both conditions are true.
        /// </summary>
        And,
        /// <summary>
        /// This is the OR operator. Combines two conditions together. Selects a record if at least one condition is true.
        /// </summary>
        Or,
        /// <summary>
        /// This is the NOT operator. Selects a record if it doesn't match the following expression.
        /// </summary>
        Not
    }

    /// <summary>
    /// Enumeration for the location of the wildcard operation in regards to the keywords.
    /// </summary>
    public enum SearchPattern
    {
        /// <summary>
        /// The wildcard operator will be placed at the start of the keyword.
        /// </summary>
        StartsWith,
        /// <summary>
        /// The wildcard operator will be placed at the start and end of the keyword.
        /// </summary>
        Contains,
        /// <summary>
        /// The wildcard operator will be placed only on the end of the keyword.
        /// </summary>
        EndsWith
    }

    /// <summary>
    /// Enumeration for changing the sql format from upper, lower or none.
    /// </summary>
    public enum TextFormatting
    {
        /// <summary>
        /// The format will be using the UPPER function for wildcard matches.
        /// </summary>
        Default,
        /// <summary>
        /// The format will be using the uppercase keyword for an exact and wildcard matches.
        /// </summary>
        Upper,
        /// <summary>
        /// The format will be using the lowercase keyword for an exact and wildcard matches.
        /// </summary>
        Lower
    }

    #endregion

    /// <summary>
    /// A supporting class used to build SQL statements based on field inputs across multiple workspaces.
    /// </summary>
    public class QueryBuilder
    {
        #region Fields

        private IObjectClass _BuildClass;
        private SearchPattern _SearchPattern;
        private SqlSyntax _SqlSyntax;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <param name="buildClass">The build class.</param>
        public QueryBuilder(IObjectClass buildClass)
            : this(buildClass, SearchPattern.StartsWith)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <param name="buildClass">The build class.</param>
        /// <param name="searchPattern">The search pattern.</param>
        public QueryBuilder(IObjectClass buildClass, SearchPattern searchPattern)
        {
            _SqlSyntax = new SqlSyntax(((IDataset)buildClass).Workspace);
            _BuildClass = buildClass;
            _SearchPattern = searchPattern;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the search pattern.
        /// </summary>
        /// <value>The search pattern.</value>
        public SearchPattern Pattern
        {
            get
            {
                return _SearchPattern;
            }
            set
            {
                _SearchPattern = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Builds the query based on the specified fields. When more then one field is available the query will use the specified logical operator
        /// to concatenate the statements.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="useWildcards">if set to <c>true</c> use wildcards.</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <returns>
        /// The SQL where clause for the specified field and workspace.
        /// </returns>
        /// <remarks>
        /// The subtype values will be extracted for the field automatically.
        /// </remarks>
        public IQueryFilter Build(IFields fields, string criteria, bool useWildcards, LogicalOperator logicalOperator)
        {
            StringBuilder builder = new StringBuilder();

            // Iterate through each field creating an sql statement for each field.
            for (int i = 0; i < fields.FieldCount; i++)
            {
                // Build the statement using the given field.
                StringBuilder statement = this.Build(fields.get_Field(i), criteria, useWildcards, logicalOperator);

                // Append the logical operator when the statement and expression length is longer then 0.
                if (builder.Length > 0 && statement.Length > 0)
                    builder.Append(string.Format(" {0} ", logicalOperator));

                // Add the statement.
                builder.Append(statement);
            }

            // Return null when nothing was built.
            if (string.IsNullOrEmpty(builder.ToString()))
                return null;

            // Return to the query filter.
            return new QueryFilterClass
            {
                WhereClause = builder.ToString()
            };
        }

        /// <summary>
        /// Builds the SQL statement based on the specified field using the logical operator to concatenate the statements.
        /// The subtype values will be extracted for the field automatically.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="useWildcards">if set to <c>true</c> use wildcards.</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <returns>
        /// The SQL where clause for the specified field and workspace.
        /// </returns>
        /// <remarks>
        /// The subtype values will be extracted for the field automatically.
        /// </remarks>
        public StringBuilder Build(IField field, string criteria, bool useWildcards, LogicalOperator logicalOperator)
        {
            return this.Build(field, criteria, useWildcards, logicalOperator, TextFormatting.Default);
        }

        /// <summary>
        /// Builds the SQL statement based on the specified field using the logical operator to concatenate the statements.        
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="useWildcards">if set to <c>true</c> [use wildcards].</param>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="textFormatting">The text formatting.</param>
        /// <returns>
        /// The SQL where clause for the specified field and workspace.
        /// </returns>
        /// <remarks>
        /// The subtype values will be extracted for the field automatically.
        /// </remarks>
        public StringBuilder Build(IField field, string criteria, bool useWildcards, LogicalOperator logicalOperator, TextFormatting textFormatting)
        {
            StringBuilder builder = new StringBuilder();

            // When the field is not supported exit out.
            if (!this.IsSupported(field.Type))
                return builder;

            // Determine the comparision operator.
            ComparisonOperator comparisonOperator = (useWildcards) ? ComparisonOperator.Like : ComparisonOperator.Equals;
            string lo = string.Empty;

            // When there are subtypes on the field we need to translate them into the coded values.
            List<string> values = this.GetDomainValues(field, criteria, useWildcards);

            // Iterate through each domain value.
            foreach (string v in values)
            {
                // Append the logical operator and the expression.
                builder.Append(lo);

                // Format the expression into an SQL statement.
                string expression = this.FormatExpression(comparisonOperator, v, field, textFormatting);
                if (string.IsNullOrEmpty(expression)) continue;

                builder.Append(expression);

                // Set the logical operator.
                lo = string.Format(" {0} ", logicalOperator);
            }

            return builder;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Produce the actual SQL string for the specified <see cref="ComparisonOperator"/>. This is the part
        /// of the SQL string between the column name and the parameter value.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="isValueNull">This parameter indicates whether the value of the parameter is null. This
        /// is required because different operators must be used for null equality checks.</param>
        /// <returns>
        /// The SQL string for the specified operator
        /// </returns>
        /// <exception cref="NotSupportedException">Unable to format query string for unknown operator.</exception>
        private string AsOperatorBegin(ComparisonOperator comparisonOperator, bool isValueNull)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equals:
                    return isValueNull ? "Is" : "=";
                case ComparisonOperator.NotEquals:
                    return "Is Not";
                case ComparisonOperator.GreaterThan:
                    return ">";
                case ComparisonOperator.GreaterThanOrEquals:
                    return ">=";
                case ComparisonOperator.LessThan:
                    return "<";
                case ComparisonOperator.LessThanOrEquals:
                    return "<=";
                case ComparisonOperator.Like:
                    return "Like";
                case ComparisonOperator.NotLike:
                    return "Not Like";
                case ComparisonOperator.In:
                    return "In";
                case ComparisonOperator.NotIn:
                    return "Not In";
                default:
                    throw new NotSupportedException("Unable to format query string for unknown operator.");
            }
        }

        /// <summary>
        /// Produce the actual SQL string for the specified <see cref="ComparisonOperator"/>. This is the part
        /// of the SQL string after the parameter value. This string is usually empty.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <returns>
        /// The SQL string for the specified operator
        /// </returns>
        private string AsOperatorEnd(ComparisonOperator comparisonOperator)
        {
            if (comparisonOperator == ComparisonOperator.In || comparisonOperator == ComparisonOperator.NotIn)
                return ")";

            return "";
        }

        /// <summary>
        /// Creates the correct SQL formatted expression for the given parameters.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="field">The field.</param>
        /// <param name="textFormatting">The text formatting.</param>
        /// <returns>
        /// The formatted SQL expression; otherwise <see cref="String.Empty"/>.
        /// </returns>
        private string FormatExpression(ComparisonOperator comparisonOperator, string criteria, IField field, TextFormatting textFormatting)
        {
            StringBuilder expression = new StringBuilder();
            expression.Append("(");

            // Change the value based on null value.
            bool isValueNull = string.IsNullOrEmpty(criteria);
            string value = (isValueNull) ? "Null" : criteria;

            // Determine the formatted value.
            string formattedValue = this.GetFormattedValue(field, value, isValueNull, textFormatting);

            // When the formatting fails exit with a null string.
            if (string.IsNullOrEmpty(formattedValue))
                return string.Empty;

            // When we are doing a like comparsion convert pattern the expression accordining to the pattern.
            if (comparisonOperator == ComparisonOperator.Like)
            {
                string patternExpression = this.PatternExpression(formattedValue, field, isValueNull, textFormatting);
                expression.Append(patternExpression);
            }
            else
            {
                // Add the field then comparison operator.
                expression.Append(field.Name);
                expression.Append(" ");
                expression.Append(this.AsOperatorBegin(comparisonOperator, isValueNull));
                expression.Append(" ");

                if (this.IsCharacter(field))
                {
                    if (isValueNull)
                        expression.Append(formattedValue);
                    else
                        expression.AppendFormat("'{0}'", formattedValue);
                }
                else
                {
                    expression.Append(formattedValue);
                }

                expression.Append(this.AsOperatorEnd(comparisonOperator));
            }

            expression.Append(")");

            return expression.ToString();
        }

        /// <summary>
        /// Gets the corresponding domain value for the value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="useWildCard">if set to <c>true</c> use wild card.</param>
        /// <returns>
        /// The code value for the domain; otherwise the value parameter.
        /// </returns>
        private List<string> GetDomainValues(IField field, string criteria, bool useWildCard)
        {
            List<string> items = new List<string>();

            // Determine if the object class supports subtypes.
            if (!(_BuildClass is ISubtypes))
            {
                items.Add(criteria);
                return items;
            }

            ISubtypes subtypes = (ISubtypes)_BuildClass;
            IEnumSubtype enumSubtypes = subtypes.Subtypes;

            enumSubtypes.Reset();
            int subtypeCD;
            string subtypeName;

            // Iterate through all of the subtypes because there can be different domains depending on the subtype.
            while ((subtypeName = enumSubtypes.Next(out subtypeCD)) != null)
            {
                if (subtypes.SubtypeFieldName.Equals(field.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    // Determine if the keyword matches the subtype name.
                    if (this.IsMatch(criteria, subtypeName, useWildCard))
                    {
                        // Add the subtype value.
                        if (!items.Contains(subtypeCD.ToString()))
                            items.Add(subtypeCD.ToString());
                    }
                }
                else
                {
                    // Determine if the field has a domain for the given subtype.
                    ICodedValueDomain domain = subtypes.get_Domain(subtypeCD, field.Name) as ICodedValueDomain;
                    if (domain != null)
                    {
                        for (int i = 0; i < domain.CodeCount; i++)
                        {
                            string name = domain.get_Name(i);

                            // Determine if the keyword matches the domain name.
                            if (this.IsMatch(criteria, name, useWildCard))
                            {
                                // Add the domain value.
                                string value = domain.get_Value(i).ToString();
                                if (!items.Contains(value))
                                    items.Add(value);
                            }
                        }
                    }
                }
            }

            // Default to the keyword.
            if (items.Count == 0)
                items.Add(criteria);

            return items;
        }

        /// <summary>
        /// Gets the formatted value for the <see cref="IField"/>.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="isValueNull">if set to <c>true</c> is value null.</param>
        /// <param name="textFormatting">The text formatting.</param>
        /// <returns>
        /// The formatted value; otherwise <see cref="String.Empty"/>
        /// </returns>
        private string GetFormattedValue(IField field, string value, bool isValueNull, TextFormatting textFormatting)
        {
            string formattedValue = string.Empty;

            // When the field is a character we can return the given value.
            if (this.IsCharacter(field))
            {
                if (isValueNull)
                {
                    formattedValue = value;
                }
                else
                {
                    formattedValue = string.Format("{0}", _SqlSyntax.Escape(value));
                }
            }
            else
            {
                // Depending on the field type perform different formatting.
                switch (field.Type)
                {
                    case esriFieldType.esriFieldTypeDate:

                        DateTime dateTime;
                        if (DateTime.TryParse(value, out dateTime))
                        {
                            formattedValue = _SqlSyntax.GetFormattedDate(dateTime);
                        }
                        else if (isValueNull)
                        {
                            formattedValue = value;
                        }
                        else
                        {
                            formattedValue = string.Format("{0}", _SqlSyntax.Escape(value));
                        }
                        break;

                    case esriFieldType.esriFieldTypeDouble:

                        double result;
                        if (double.TryParse(value, out result))
                        {
                            formattedValue = string.Format("{0}", value);
                        }
                        break;

                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeOID:
                    case esriFieldType.esriFieldTypeSingle:
                    case esriFieldType.esriFieldTypeSmallInteger:

                        int number;
                        if (int.TryParse(value, out number))
                        {
                            formattedValue = string.Format("{0}", number);
                        }

                        break;
                    default:
                        break;
                }
            }

            // Depending on the formatting upper or lower case the value.
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            switch (textFormatting)
            {
                case TextFormatting.Upper:
                    return textInfo.ToUpper(formattedValue);
                case TextFormatting.Lower:
                    return textInfo.ToLower(formattedValue);
                default:
                    return formattedValue;
            }
        }

        /// <summary>
        /// Determines whether the specified field is a character
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        /// 	<c>true</c> if the specified field type is a character; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCharacter(IField field)
        {
            switch (field.Type)
            {
                case esriFieldType.esriFieldTypeGUID:
                case esriFieldType.esriFieldTypeGlobalID:
                case esriFieldType.esriFieldTypeString:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified keyword matches the value
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value">The value.</param>
        /// <param name="useWildCard">if set to <c>true</c> use wild card.</param>
        /// <returns>
        /// 	<c>true</c> if the specified keyword matches the value; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMatch(string criteria, string value, bool useWildCard)
        {
            if (useWildCard)
            {
                switch (this.Pattern)
                {
                    default:
                        return value.StartsWith(criteria, StringComparison.CurrentCultureIgnoreCase);

                    case SearchPattern.Contains:
                        return value.Contains(criteria);

                    case SearchPattern.EndsWith:
                        return value.EndsWith(criteria, StringComparison.CurrentCultureIgnoreCase);
                }
            }

            if (value.Equals(criteria, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified field type is supported.
        /// </summary>
        /// <param name="value">Type of the field.</param>
        /// <returns>
        /// 	<c>true</c> if the specified field type is supported; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSupported(esriFieldType value)
        {
            switch (value)
            {
                case esriFieldType.esriFieldTypeBlob:
                case esriFieldType.esriFieldTypeGeometry:
                case esriFieldType.esriFieldTypeRaster:
                case esriFieldType.esriFieldTypeXML:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Gets the formatted expression in regards to the <see cref="SearchPattern"/> enumeration.
        /// </summary>
        /// <param name="formattedValue">The formatted value.</param>
        /// <param name="field">The field.</param>
        /// <param name="isValueNull">if set to <c>true</c> [is value null].</param>
        /// <param name="textFormatting">The text formatting.</param>
        /// <returns>
        /// The formatted SQL expression for the value and field.
        /// </returns>
        /// <remarks>There are performance considerations in regards to file geodatabase vs remote geodatabase. File geodatabases don't support
        /// function based indexes which can hurt performance when using UPPER or CAST because it requires a full table scan, this can cause
        /// extremely poor performance on large tables. Observed a 20 minute search on a table with 2 Million records for a single match.</remarks>
        private string PatternExpression(string formattedValue, IField field, bool isValueNull, TextFormatting textFormatting)
        {
            string wildcard = _SqlSyntax.GetSpecialCharacter(esriSQLSpecialCharacters.esriSQL_WildcardManyMatch);
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            // Depending on the field we format the SQL differently.
            if (this.IsCharacter(field))
            {
                if (isValueNull)
                    return string.Format("{0} {1} '{2}'", field.Name, ComparisonOperator.Like, wildcard);

                // Depending on the format we will alter the SQL statement to use or exclude the functions.
                switch (textFormatting)
                {
                    // Perform the search using the functions.
                    default:

                        string upper = _SqlSyntax.GetFunction(esriSQLFunctionName.esriSQL_UPPER);

                        // Depending on the pattern we need to place the wildcard in a different location.
                        switch (this.Pattern)
                        {
                            default:
                                return string.Format("{0}({1}) {2} '{3}{4}'", upper, field.Name, ComparisonOperator.Like, textInfo.ToUpper(formattedValue), wildcard);

                            case SearchPattern.Contains:
                                return string.Format("{0}({1}) {2} '{4}{3}{4}'", upper, field.Name, ComparisonOperator.Like, textInfo.ToUpper(formattedValue), wildcard);

                            case SearchPattern.EndsWith:
                                return string.Format("{0}({1}) {2} '{4}{3}'", upper, field.Name, ComparisonOperator.Like, textInfo.ToUpper(formattedValue), wildcard);
                        }

                    // Perform the search not using the functions and properly case the formatted value.
                    case TextFormatting.Upper:
                    case TextFormatting.Lower:

                        // Depending on the pattern we need to place the wildcard in a different location.
                        switch (this.Pattern)
                        {
                            default:
                                return string.Format("{0} {1} '{2}{3}'", field.Name, ComparisonOperator.Like, formattedValue, wildcard);

                            case SearchPattern.Contains:
                                return string.Format("{0} {1} '{3}{2}{3}'", field.Name, ComparisonOperator.Like, formattedValue, wildcard);

                            case SearchPattern.EndsWith:
                                return string.Format("{0} {1} '{3}{2}'", field.Name, ComparisonOperator.Like, formattedValue, wildcard);
                        }
                }
            }

            // Microsoft Jet Driver doesn't support the same function casting as the other datatabase,
            // thus we need to reformat the non character statement.
            if (_SqlSyntax.Type == SpatialDataEngine.Access)
            {
                // We use the the CSTR statement for Access appended with "" because if the value is null it will return an empty string instead avoiding
                // an Invalid use of 'Null' error that will occur when the field value is null.
                if (isValueNull)
                    return string.Format("CSTR({0} & \"\") {1} '{2}'", field.Name, ComparisonOperator.Like, wildcard);

                // Depending on the pattern we need to place the wildcard in a different location.
                switch (this.Pattern)
                {
                    default:
                        return string.Format("CSTR({0} & \"\") {1} '{2}{3}'", field.Name, ComparisonOperator.Like, formattedValue, wildcard);

                    case SearchPattern.Contains:
                        return string.Format("CSTR({0} & \"\") {1} '{3}{2}{3}'", field.Name, ComparisonOperator.Like, formattedValue, wildcard);

                    case SearchPattern.EndsWith:
                        return string.Format("CSTR({0} & \"\") {1} '{3}{2}'", field.Name, ComparisonOperator.Like, formattedValue, wildcard);
                }
            }
            else
            {
                // Use the CHAR cast statement which supported in all databases except Access.
                string cast = _SqlSyntax.GetFunction(esriSQLFunctionName.esriSQL_CAST);

                if (isValueNull)
                    return string.Format("{0}({1} As CHAR({2})) {3} '{4}'", cast, field.Name, field.Name.Length, ComparisonOperator.Like, wildcard);

                // Depending on the pattern we need to place the wildcard in a different location.
                switch (this.Pattern)
                {
                    default:
                        return string.Format("{0}({1} As CHAR({2})) {3} '{4}{5}'", cast, field.Name, field.Name.Length, ComparisonOperator.Like, formattedValue, wildcard);

                    case SearchPattern.Contains:
                        return string.Format("{0}({1} As CHAR({2})) {3} '{5}{4}{5}'", cast, field.Name, field.Name.Length, ComparisonOperator.Like, formattedValue, wildcard);

                    case SearchPattern.EndsWith:
                        return string.Format("{0}({1} As CHAR({2})) {3} '{5}{4}'", cast, field.Name, field.Name.Length, ComparisonOperator.Like, formattedValue, wildcard);
                }
            }
        }

        #endregion
    }
}