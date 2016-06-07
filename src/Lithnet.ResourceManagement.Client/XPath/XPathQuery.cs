using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices;
using Lithnet.ResourceManagement.Client;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Lithnet.ResourceManagement.Client
{
    /*
             FROM https://msdn.microsoft.com/en-us/library/windows/desktop/ee652287(v=vs.100).aspx
            
               Given the following string: "The quick brown fox," and the contains() query on the string "u", the expected result is 
               that nothing is returned, since the letter "u" only appears in the middle of a string, and not immediately after a word-breaker. 
               If we ran the contains() query on the string "qu", however, we would get a match, since the substring "qu" appears immediately 
               after a word-breaking character.
          
    */

    /// <summary>
    /// Represents a predicate within an XPath expression that is used for comparing an attribute with a value
    /// </summary>
    public class XPathQuery : IXPathQueryObject
    {
        /// <summary>
        /// The type of the attribute used in the query
        /// </summary>
        private AttributeType attributeType;

        /// <summary>
        /// The mutivalued status of the attribute used in the query
        /// </summary>
        private bool isMultivalued;

        /// <summary>
        /// The maximum value of an integer that the Resource Management Service web service seems to be able to parse
        /// Technically this should be long.MaxValue, but that throws a parsing error in the web service
        /// </summary>
        internal const long MaxLong = 999999999999999L;

        /// <summary>
        /// The maximum value of a date time in the Resource Management Service
        /// </summary>
        internal const string MaxDate = "9999-12-31T23:59:59.997";

        /// <summary>
        /// Gets a value indicating if the condition in this query will be negated with the not() operator
        /// </summary>
        public bool Negate { get; private set; }

        /// <summary>
        /// Gets the value comparison operator used in the query
        /// </summary>
        public ComparisonOperator Operator { get; private set; }

        /// <summary>
        /// Gets the name of the attribute used in the query
        /// </summary>
        public string AttributeName { get; private set; }

        /// <summary>
        /// Gets the value that is used in the query
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the XPathQuery class
        /// </summary>
        /// <param name="attributeName">The name of the attribute to compare against</param>
        /// <param name="comparisonOperator">The value comparison operator to use</param>
        /// <remarks>
        /// This constructor only supports the use of the <c>ComparisonOperator.IsPresent</c> and <c>ComparisonOperator.NotPresent</c> values
        /// </remarks>
        public XPathQuery(string attributeName, ComparisonOperator comparisonOperator)
            : this(attributeName, comparisonOperator, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XPathQuery class
        /// </summary>
        /// <param name="attributeName">The name of the attribute to compare against</param>
        /// <param name="comparisonOperator">The value comparison operator to use</param>
        /// <param name="value">The value to compare against</param>
        public XPathQuery(string attributeName, ComparisonOperator comparisonOperator, object value)
            : this(attributeName, comparisonOperator, value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XPathQuery class
        /// </summary>
        /// <param name="attributeName">The name of the attribute to compare against</param>
        /// <param name="comparisonOperator">The value comparison operator to use</param>
        /// <param name="value">The value to compare against</param>
        /// <param name="negate">Indicates if this query should be negated with the not() operator</param>
        public XPathQuery(string attributeName, ComparisonOperator comparisonOperator, object value, bool negate)
        {
            AttributeType attributeType = ResourceManagementSchema.GetAttributeType(attributeName);
            bool isMultivalued = ResourceManagementSchema.IsAttributeMultivalued(attributeName);

            this.SetupBuilder(attributeName, comparisonOperator, value, negate, attributeType, isMultivalued);

        }

        /// <summary>
        /// Initializes a new instance of the XPathQuery class
        /// </summary>
        /// <param name="attributeName">The name of the attribute to compare against</param>
        /// <param name="comparisonOperator">The value comparison operator to use</param>
        /// <param name="value">The value to compare against</param>
        /// <param name="negate">Indicates if this query should be negated with the not() operator</param>
        /// <param name="attributeType">The data type of the attribute being queried</param>
        /// <param name="isMultivalued">The multivalued status of the attribute being queried</param>
        /// <remarks>
        /// This constructor can be used when a connection to the resource management service not available. The attribute type and multivalued status are not validated against the schema
        /// </remarks>
        public XPathQuery(string attributeName, ComparisonOperator comparisonOperator, object value, bool negate, AttributeType attributeType, bool isMultivalued)
        {
            this.SetupBuilder(attributeName, comparisonOperator, value, negate, attributeType, isMultivalued);
        }

        /// <summary>
        /// Sets up the query object and validates the passed in parameters to ensure they are compatible with each other
        /// </summary>
        /// <param name="attributeName">The name of the attribute being queried</param>
        /// <param name="comparisonOperator">THe value comparison operator to use</param>
        /// <param name="value">The value to use in the query</param>
        /// <param name="negate">Indicates if the query should be negated with the not() operator</param>
        /// <param name="attributeType">The type of the target attribute</param>
        /// <param name="isMultivalued">The multivalued status of the attribute being queried</param>
        private void SetupBuilder(string attributeName, ComparisonOperator comparisonOperator, object value, bool negate, AttributeType attributeType, bool isMultivalued)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentNullException(attributeName);
            }

            ResourceManagementSchema.ValidateAttributeName(attributeName);

            if (value == null)
            {
                if (comparisonOperator != ComparisonOperator.IsNotPresent && comparisonOperator != ComparisonOperator.IsPresent)
                {
                    throw new InvalidOperationException("An object value is required unless the operator is IsPresent or IsNotPresent");
                }
            }

            this.AttributeName = attributeName;
            this.Operator = comparisonOperator;
            this.Value = value;
            this.Negate = negate;

            this.attributeType = attributeType;
            this.isMultivalued = isMultivalued;

            this.ThrowOnInvalidTypeOperatorCombination();
            this.ThrowOnInvalidNegateCombination();
        }

        /// <summary>
        /// Builds the query component
        /// </summary>
        /// <returns>A string containing the query</returns>
        public string BuildQueryString()
        {
            return this.ToString();
        }

        /// <summary>
        /// Gets the string representation of this object
        /// </summary>
        /// <returns>A string containing the query</returns>
        public override string ToString()
        {
            return this.BuildXpathPredicate();
        }

        /// <summary>
        /// Calls the appropriate function to build the predicate
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string BuildXpathPredicate()
        {
            switch (this.Operator)
            {
                case ComparisonOperator.Equals:
                    return this.GetExpressionEquals();

                case ComparisonOperator.NotEquals:
                    return this.GetExpressionNotEquals();

                case ComparisonOperator.GreaterThan:
                    return this.GetExpressionGreaterThan();

                case ComparisonOperator.GreaterThanOrEquals:
                    return this.GetExpressionGreaterThanOrEquals();

                case ComparisonOperator.LessThan:
                    return this.GetExpressionLessThan();

                case ComparisonOperator.LessThanOrEquals:
                    return this.GetExpressionLessThanOrEquals();

                case ComparisonOperator.IsPresent:
                    return this.GetExpressionIsPresent();

                case ComparisonOperator.IsNotPresent:
                    return this.GetExpressionIsNotPresent();

                case ComparisonOperator.Contains:
                    return this.GetExpressionContains();

                case ComparisonOperator.StartsWith:
                    return this.GetExpressionStartsWith();

                case ComparisonOperator.EndsWith:
                    return this.GetExpressionEndsWith();

                default:
                    throw new NotSupportedException("The operator was unknown");
            }
        }

        /// <summary>
        /// Builds the predicate for an equals comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionEquals()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} = {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else if (this.attributeType == AttributeType.Boolean)
            {
                expression = string.Format("({0} = {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else if (this.attributeType == AttributeType.Reference)
            {
                string valuetoUse;

                XPathExpression childExpression = this.Value as XPathExpression;

                if (childExpression != null)
                {
                    valuetoUse = childExpression.ToString();
                }
                else
                {
                    valuetoUse = string.Format("'{0}'", TypeConverter.ToUniqueIdentifier(this.Value).Value);
                }

                expression = string.Format("({0} = {1})", this.AttributeName, valuetoUse);
            }
            else if (this.attributeType == AttributeType.DateTime)
            {
                expression = string.Format("({0} = {1})", this.AttributeName, TypeConverter.ToString(this.QuoteIfNotFunction(this.Value)));
            }
            else
            {
                expression = string.Format("({0} = {1})", this.AttributeName, this.QuoteTextValue(TypeConverter.ToString(this.Value)));
            }

            return ProcessNegation(expression);
        }

        /// <summary>
        /// Builds the predicate for a not equals comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionNotEquals()
        {
            if (this.attributeType == AttributeType.Integer || this.attributeType == AttributeType.Boolean)
            {
                return string.Format("(not({0} = {1}))", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else if (this.attributeType == AttributeType.Reference)
            {
                string valuetoUse;

                XPathExpression childExpression = this.Value as XPathExpression;

                if (childExpression != null)
                {
                    valuetoUse = childExpression.ToString();
                }
                else
                {
                    valuetoUse = string.Format("'{0}'", TypeConverter.ToUniqueIdentifier(this.Value).Value);
                }

                return string.Format("(not({0} = {1}))", this.AttributeName, valuetoUse);
            }
            else if (this.attributeType == AttributeType.DateTime)
            {
                return string.Format("(not({0} = {1}))", this.AttributeName, TypeConverter.ToString(this.QuoteIfNotFunction(this.Value)));
            }
            else
            {
                return string.Format("(not({0} = {1}))", this.AttributeName, this.QuoteTextValue(TypeConverter.ToString(this.Value)));
            }
        }

        /// <summary>
        /// Builds the predicate for a greater than comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionGreaterThan()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} > {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else if (this.attributeType == AttributeType.DateTime)
            {
                return string.Format("({0} > {1})", this.AttributeName, TypeConverter.ToString(this.QuoteIfNotFunction(this.Value)));
            }
            else
            {
                expression = string.Format("({0} > '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        /// <summary>
        /// Builds the predicate for a greater than or equals to comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionGreaterThanOrEquals()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} >= {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else if (this.attributeType == AttributeType.DateTime)
            {
                return string.Format("({0} >= {1})", this.AttributeName, TypeConverter.ToString(this.QuoteIfNotFunction(this.Value)));
            }
            else
            {
                expression = string.Format("({0} >= '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        /// <summary>
        /// Builds the predicate for a less than comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionLessThan()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} < {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else if (this.attributeType == AttributeType.DateTime)
            {
                return string.Format("({0} < {1})", this.AttributeName, TypeConverter.ToString(this.QuoteIfNotFunction(this.Value)));
            }
            else
            {
                expression = string.Format("({0} < '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        /// <summary>
        /// Builds the predicate for a less than or equals to comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionLessThanOrEquals()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} <= {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else if (this.attributeType == AttributeType.DateTime)
            {
                return string.Format("({0} <= {1})", this.AttributeName, TypeConverter.ToString(this.QuoteIfNotFunction(this.Value)));
            }
            else
            {
                expression = string.Format("({0} <= '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        /// <summary>
        /// Builds the predicate for an attribute presence check
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionIsPresent()
        {
            if (attributeType == AttributeType.Reference)
            {
                return string.Format("({0} = /*)", this.AttributeName);
            }
            else if (attributeType == AttributeType.Integer)
            {
                return string.Format("({0} <= {1})", this.AttributeName, XPathQuery.MaxLong);
            }
            else if (attributeType == AttributeType.DateTime)
            {
                return string.Format("({0} <= '{1}')", this.AttributeName, XPathQuery.MaxDate);
            }
            else if (attributeType == AttributeType.Boolean)
            {
                return string.Format("(({0} = true) or ({0} = false))", this.AttributeName);
            }
            else
            {
                return string.Format("(starts-with({0}, '%'))", this.AttributeName);
            }
        }

        /// <summary>
        /// Builds the predicate for an attribute not present check
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionIsNotPresent()
        {
            if (attributeType == AttributeType.Reference)
            {
                return string.Format("(not({0} = /*))", this.AttributeName);
            }
            else if (attributeType == AttributeType.Integer)
            {
                return string.Format("(not({0} <= {1}))", this.AttributeName, XPathQuery.MaxLong);
            }
            else if (attributeType == AttributeType.DateTime)
            {
                return string.Format("(not({0} <= '{1}'))", this.AttributeName, XPathQuery.MaxDate);
            }
            else if (attributeType == AttributeType.Boolean)
            {
                return string.Format("(not(({0} = true) or ({0} = false)))", this.AttributeName);
            }
            else
            {
                return string.Format("(not(starts-with({0}, '%')))", this.AttributeName);
            }
        }

        /// <summary>
        /// Builds the predicate for a contains comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionContains()
        {
            return ProcessNegation(string.Format("(contains({0}, {1}))", this.AttributeName, this.QuoteTextValue(TypeConverter.ToString(this.Value))));
        }

        /// <summary>
        /// Builds the predicate for a starts with comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionStartsWith()
        {
            return ProcessNegation(string.Format("(starts-with({0}, {1}))", this.AttributeName, this.QuoteTextValue(TypeConverter.ToString(this.Value))));
        }

        /// <summary>
        /// Builds the predicate for an ends with comparison
        /// </summary>
        /// <returns>A string containing the query</returns>
        private string GetExpressionEndsWith()
        {
            return ProcessNegation(string.Format("(ends-with({0}, {1}))", this.AttributeName, this.QuoteTextValue(TypeConverter.ToString(this.Value))));
        }

        private object QuoteIfNotFunction(object value)
        {
            if (!(value is string))
            {
                // If the value is not a string, it cant be a function so convert it to a string and return the quoted value
                return string.Format("'{0}'", TypeConverter.ToString(value));
            }

            string trimmedValue = ((string)value).TrimStart();

            DateTime result;

            if (DateTime.TryParseExact((string)value, TypeConverter.FimServiceDateFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out result))
            {
                // The string was a date time value, so pass it back as a string
                return string.Format("'{0}'", result.ToResourceManagementServiceDateFormat());
            }
            else
            {
                // Value is not a date, assume it is a function and don't quote it
                return value;
            }
        }

        /// <summary>
        /// Converts the query into a not() expression
        /// </summary>
        /// <param name="expression">The expression to negate</param>
        /// <returns>A string containing the query</returns>
        private string ProcessNegation(string expression)
        {
            if (this.Negate)
            {
                expression = string.Format("(not{0})", expression);
            }

            return expression;
        }

        /// <summary>
        /// Throws an exception when an invalid data type and comparison operator is detected
        /// </summary>
        private void ThrowOnInvalidTypeOperatorCombination()
        {
            switch (this.attributeType)
            {
                case AttributeType.Binary:
                    this.ThrowOnInvalidBinaryOperator();
                    break;

                case AttributeType.Boolean:
                    this.ThrowOnInvalidBooleanOperator();
                    break;

                case AttributeType.DateTime:
                    this.ThrowOnInvalidDateTimeOperator();
                    break;

                case AttributeType.Integer:
                    this.ThrowOnInvalidIntegerOperator();
                    break;

                case AttributeType.Reference:
                    this.ThrowOnInvalidReferenceOperator();
                    break;

                case AttributeType.String:
                    this.ThrowOnInvalidStringOperator();
                    break;

                case AttributeType.Text:
                    this.ThrowOnInvalidTextOperator();
                    break;

                default:
                case AttributeType.Unknown:
                    break;
            }
        }

        /// <summary>
        /// Throws an exception when an invalid comparison operator is used with an integer data type
        /// </summary>
        private void ThrowOnInvalidIntegerOperator()
        {
            switch (this.Operator)
            {
                case ComparisonOperator.Contains:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));
                default:
                    break;
            }
        }

        /// <summary>
        /// Throws an exception when an invalid comparison operator is used with a date time data type
        /// </summary>
        private void ThrowOnInvalidDateTimeOperator()
        {
            switch (this.Operator)
            {
                case ComparisonOperator.Contains:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));
                default:
                    break;
            }
        }

        /// <summary>
        /// Throws an exception when an invalid comparison operator is used with a string data type
        /// </summary>
        private void ThrowOnInvalidStringOperator()
        {
            switch (this.Operator)
            {
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.GreaterThanOrEquals:
                case ComparisonOperator.LessThan:
                case ComparisonOperator.LessThanOrEquals:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));

                default:
                    break;
            }
        }

        /// <summary>
        /// Throws an exception when an invalid comparison operator is used with a reference data type
        /// </summary>
        private void ThrowOnInvalidReferenceOperator()
        {
            switch (this.Operator)
            {
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.GreaterThanOrEquals:
                case ComparisonOperator.LessThan:
                case ComparisonOperator.LessThanOrEquals:
                case ComparisonOperator.Contains:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));
                default:
                    break;
            }
        }

        /// <summary>
        /// Throws an exception when attempting to query a binary attribute type
        /// </summary>
        private void ThrowOnInvalidBinaryOperator()
        {
            throw new NotSupportedException("Cannot search on an attribute of type 'Binary'");
        }

        /// <summary>
        /// Throws an exception when attempting to query a text attribute type
        /// </summary>
        private void ThrowOnInvalidTextOperator()
        {
            throw new NotSupportedException("Cannot search on an attribute of type 'Text'");
        }

        /// <summary>
        /// Throws an exception when an invalid comparison operator is used with a boolean data type
        /// </summary>
        private void ThrowOnInvalidBooleanOperator()
        {
            switch (this.Operator)
            {
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.GreaterThanOrEquals:
                case ComparisonOperator.LessThan:
                case ComparisonOperator.LessThanOrEquals:
                case ComparisonOperator.Contains:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));
                default:
                    break;
            }
        }

        /// <summary>
        /// Throws an exception when trying to negate an expression that does not support negation
        /// </summary>
        private void ThrowOnInvalidNegateCombination()
        {
            if (this.Negate)
            {
                switch (this.Operator)
                {
                    case ComparisonOperator.NotEquals:
                    case ComparisonOperator.IsPresent:
                    case ComparisonOperator.IsNotPresent:
                        throw new InvalidOperationException(string.Format("Cannot negate a query with a {0} operator", this.Operator));

                    default:
                        break;
                }
            }
        }

        private string QuoteTextValue(string value)
        {
            if (value.Contains("'"))
            {
                if (value.Contains("\""))
                {
                    throw new ArgumentException("Cannot quote a value that contains both single and double quotes");
                }
                else
                {
                    return $"\"{value}\"";
                }
            }

            return $"'{value}'";
        }
    }
}