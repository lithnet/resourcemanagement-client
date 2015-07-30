using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices;

namespace Lithnet.ResourceManagement.Client
{
    public class XPathPredicate : IXPathPredicateComponent
    {
        internal static long MaxLong = 999999999999999L;

        internal static string MaxDate = "9999-12-31T23:59:59.997";

        /*
              FROM https://msdn.microsoft.com/en-us/library/windows/desktop/ee652287(v=vs.100).aspx
            
                Given the following string: "The quick brown fox," and the contains() query on the string "u", the expected result is 
                that nothing is returned, since the letter "u" only appears in the middle of a string, and not immediately after a word-breaker. 
                If we ran the contains() query on the string "qu", however, we would get a match, since the substring "qu" appears immediately 
                after a word-breaking character.
          
        */

        public bool Negate { get; private set; }

        public XPathOperator Operator { get; private set; }

        public string AttributeName { get; private set; }

        public object Value { get; private set; }

        private AttributeType attributeType;

        private bool isMultivalued;

        public XPathPredicate(string attributeName, XPathOperator xpathOperator)
            : this(attributeName, xpathOperator, null, false)
        {
        }

        public XPathPredicate(string attributeName, XPathOperator xpathOperator, object value)
            : this(attributeName, xpathOperator, value, false)
        {
        }

        public XPathPredicate(string attributeName, XPathOperator xpathOperator, object value, bool negate)
        {
            AttributeType attributeType = ResourceManagementSchema.GetAttributeType(attributeName);
            bool isMultivalued = ResourceManagementSchema.IsAttributeMultivalued(attributeName);

            this.SetupBuilder(attributeName, xpathOperator, value, negate, attributeType, isMultivalued);

        }

        public XPathPredicate(string attributeName, XPathOperator xpathOperator, object value, bool negate, AttributeType attributeType, bool isMultivalued)
        {
            this.SetupBuilder(attributeName, xpathOperator, value, negate, attributeType, isMultivalued);
        }

        private void SetupBuilder(string attributeName, XPathOperator xpathOperator, object value, bool negate, AttributeType attributeType, bool isMultivalued)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentNullException(attributeName);
            }

            if (value == null)
            {
                if (xpathOperator != XPathOperator.IsNotPresent && xpathOperator != XPathOperator.IsPresent)
                {
                    throw new InvalidOperationException("An object value is required unless the operator is IsPresent or IsNotPresent");
                }
            }

            this.AttributeName = attributeName;
            this.Operator = xpathOperator;
            this.Value = value;
            this.Negate = negate;

            this.attributeType = attributeType;
            this.isMultivalued = isMultivalued;

            this.ThrowOnInvalidTypeOperatorCombination();
            this.ThrowOnInvalidNegateCombination();
        }

        public override string ToString()
        {
            return this.BuildXpathPredicate();
        }

        private string BuildXpathPredicate()
        {
            switch (this.Operator)
            {
                case XPathOperator.Equals:
                    return this.GetExpressionEquals();

                case XPathOperator.NotEquals:
                    return this.GetExpressionNotEquals();

                case XPathOperator.GreaterThan:
                    return this.GetExpressionGreaterThan();

                case XPathOperator.GreaterThanOrEquals:
                    return this.GetExpressionGreaterThanOrEquals();

                case XPathOperator.LessThan:
                    return this.GetExpressionLessThan();

                case XPathOperator.LessThanOrEquals:
                    return this.GetExpressionLessThanOrEquals();

                case XPathOperator.IsPresent:
                    return this.GetExpressionIsPresent();

                case XPathOperator.IsNotPresent:
                    return this.GetExpressionIsNotPresent();

                case XPathOperator.Contains:
                    return this.GetExpressionContains();

                case XPathOperator.StartsWith:
                    return this.GetExpressionStartsWith();

                case XPathOperator.EndsWith:
                    return this.GetExpressionEndsWith();

                default:
                    throw new NotSupportedException("The operator was unknown");
            }
        }

        private string GetExpressionEquals()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer || this.attributeType == AttributeType.Boolean)
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
                    valuetoUse = string.Format("'{0}'", ((UniqueIdentifier)TypeConverter.ToString(this.Value)).Value);
                }

                expression = string.Format("({0} = {1})", this.AttributeName, valuetoUse);
            }
            else
            {
                expression = string.Format("({0} = '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        private string GetExpressionNotEquals()
        {
            if (this.isMultivalued)
            {
                if (this.attributeType == AttributeType.Integer || this.attributeType == AttributeType.Boolean)
                {
                    return string.Format("(not({0} = {1}))", this.AttributeName, TypeConverter.ToString(this.Value));
                }
                else if (this.attributeType == AttributeType.Reference)
                {
                    return string.Format("(not({0} = '{1}'))", this.AttributeName, ((UniqueIdentifier)TypeConverter.ToString(this.Value)).Value);
                }
                else
                {
                    return string.Format("(not({0} = '{1}'))", this.AttributeName, TypeConverter.ToString(this.Value));
                }
            }
            else
            {
                if (this.attributeType == AttributeType.Integer || this.attributeType == AttributeType.Boolean)
                {
                    return string.Format("({0} != {1})", this.AttributeName, TypeConverter.ToString(this.Value));
                }
                else if (this.attributeType == AttributeType.Reference)
                {
                    return string.Format("(not({0} = '{1}'))", this.AttributeName, ((UniqueIdentifier)TypeConverter.ToString(this.Value)).Value);
                }
                else
                {
                    return string.Format("({0} != '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
                }
            }
        }

        private string GetExpressionGreaterThan()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} > {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else
            {
                expression = string.Format("({0} > '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        private string GetExpressionGreaterThanOrEquals()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} >= {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else
            {
                expression = string.Format("({0} >= '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        private string GetExpressionLessThan()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} < {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else
            {
                expression = string.Format("({0} < '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        private string GetExpressionLessThanOrEquals()
        {
            string expression;

            if (this.attributeType == AttributeType.Integer)
            {
                expression = string.Format("({0} <= {1})", this.AttributeName, TypeConverter.ToString(this.Value));
            }
            else
            {
                expression = string.Format("({0} <= '{1}')", this.AttributeName, TypeConverter.ToString(this.Value));
            }

            return ProcessNegation(expression);
        }

        private string GetExpressionIsPresent()
        {
            if (attributeType == AttributeType.Reference)
            {
                return string.Format("({0} = /*)", this.AttributeName);
            }
            else if (attributeType == AttributeType.Integer)
            {
                return string.Format("({0} <= {1})", this.AttributeName, XPathPredicate.MaxLong);
            }
            else if (attributeType == AttributeType.DateTime)
            {
                return string.Format("({0} <= '{1}')", this.AttributeName, XPathPredicate.MaxDate);
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

        private string GetExpressionIsNotPresent()
        {
            if (attributeType == AttributeType.Reference)
            {
                return string.Format("(not({0} = /*))", this.AttributeName);
            }
            else if (attributeType == AttributeType.Integer)
            {
                return string.Format("(not({0} <= {1}))", this.AttributeName, XPathPredicate.MaxLong);
            }
            else if (attributeType == AttributeType.DateTime)
            {
                return string.Format("(not({0} <= '{1}'))", this.AttributeName, XPathPredicate.MaxDate);
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

        private string GetExpressionContains()
        {
            return ProcessNegation(string.Format("(contains({0}, '{1}'))", this.AttributeName, TypeConverter.ToString(this.Value)));
        }

        private string GetExpressionStartsWith()
        {
            return ProcessNegation(string.Format("(starts-with({0}, '{1}'))", this.AttributeName, TypeConverter.ToString(this.Value)));
        }

        private string GetExpressionEndsWith()
        {
            return ProcessNegation(string.Format("(ends-with({0}, '{1}'))", this.AttributeName, TypeConverter.ToString(this.Value)));
        }

        private string ProcessNegation(string expression)
        {
            if (this.Negate)
            {
                expression = string.Format("(not{0})", expression);
            }

            return expression;
        }

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

        private void ThrowOnInvalidIntegerOperator()
        {
            switch (this.Operator)
            {
                case XPathOperator.Contains:
                case XPathOperator.StartsWith:
                case XPathOperator.EndsWith:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));
                default:
                    break;
            }
        }

        private void ThrowOnInvalidDateTimeOperator()
        {
            switch (this.Operator)
            {
                case XPathOperator.Contains:
                case XPathOperator.StartsWith:
                case XPathOperator.EndsWith:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));
                default:
                    break;
            }
        }

        private void ThrowOnInvalidStringOperator()
        {
            switch (this.Operator)
            {
                case XPathOperator.GreaterThan:
                case XPathOperator.GreaterThanOrEquals:
                case XPathOperator.LessThan:
                case XPathOperator.LessThanOrEquals:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));

                default:
                    break;
            }
        }

        private void ThrowOnInvalidReferenceOperator()
        {
            switch (this.Operator)
            {
                case XPathOperator.GreaterThan:
                case XPathOperator.GreaterThanOrEquals:
                case XPathOperator.LessThan:
                case XPathOperator.LessThanOrEquals:
                case XPathOperator.Contains:
                case XPathOperator.StartsWith:
                case XPathOperator.EndsWith:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));
                default:
                    break;
            }
        }

        private void ThrowOnInvalidBinaryOperator()
        {
            throw new NotSupportedException("Cannot filter on a binary attributeName");
        }

        private void ThrowOnInvalidTextOperator()
        {
            throw new NotSupportedException("Cannot filter on a text attributeName");
        }

        private void ThrowOnInvalidBooleanOperator()
        {
            switch (this.Operator)
            {
                case XPathOperator.GreaterThan:
                case XPathOperator.GreaterThanOrEquals:
                case XPathOperator.LessThan:
                case XPathOperator.LessThanOrEquals:
                case XPathOperator.Contains:
                case XPathOperator.StartsWith:
                case XPathOperator.EndsWith:
                    throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", this.Operator, this.attributeType));
                default:
                    break;
            }
        }

        private void ThrowOnInvalidNegateCombination()
        {
            if (this.Negate)
            {
                switch (this.Operator)
                {
                    case XPathOperator.NotEquals:
                    case XPathOperator.IsPresent:
                    case XPathOperator.IsNotPresent:
                        throw new InvalidOperationException(string.Format("Cannot negate a query with a {0} operator", this.Operator));

                    default:
                        break;
                }
            }
        }
    }
}