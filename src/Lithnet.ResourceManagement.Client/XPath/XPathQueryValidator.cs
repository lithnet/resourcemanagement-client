using System;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Validates the components used to construct an XPath query.
    /// </summary>
    public static class XPathQueryValidator
    {
        /// <summary>
        /// Validates the components used to construct an XPath query.
        /// </summary>
        /// <param name="attribute">The attribute used in the query.</param>
        /// <param name="comparisonOperator">The comparison operator to use.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="negate">Indicates whether the query should be negated.</param>
        public static void Validate(AttributeTypeDefinition attribute, ComparisonOperator comparisonOperator, object value, bool negate)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            Validate(attribute.SystemName, attribute.Type, comparisonOperator, value, negate);
        }

        /// <summary>
        /// Validates the components used to construct an XPath query.
        /// </summary>
        /// <param name="attributeName">The name of the attribute used in the query.</param>
        /// <param name="attributeType">The type of the attribute used in the query.</param>
        /// <param name="comparisonOperator">The comparison operator to use.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="negate">Indicates whether the query should be negated.</param>
        public static void Validate(string attributeName, AttributeType attributeType, ComparisonOperator comparisonOperator, object value, bool negate)
        {
            ValidateAttributeName(attributeName);
            ValidateValue(comparisonOperator, value);
            ValidateTypeOperatorCombination(attributeType, comparisonOperator);
            ValidateNegateCombination(comparisonOperator, negate);
        }

        private static void ValidateAttributeName(string attributeName)
        {
            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentException("The attribute name cannot be empty or contain only whitespace.", nameof(attributeName));
            }
        }

        private static void ValidateValue(ComparisonOperator comparisonOperator, object value)
        {
            if (value == null && comparisonOperator != ComparisonOperator.IsNotPresent && comparisonOperator != ComparisonOperator.IsPresent)
            {
                throw new ArgumentNullException(nameof(value), "A value is required unless the operator is IsPresent or IsNotPresent.");
            }
        }

        private static void ValidateTypeOperatorCombination(AttributeType attributeType, ComparisonOperator comparisonOperator)
        {
            switch (attributeType)
            {
                case AttributeType.Binary:
                    throw new NotSupportedException("Cannot search on an attribute of type 'Binary'");

                case AttributeType.Boolean:
                    ValidateBooleanOperator(attributeType, comparisonOperator);
                    break;

                case AttributeType.DateTime:
                    ValidateDateTimeOperator(attributeType, comparisonOperator);
                    break;

                case AttributeType.Integer:
                    ValidateIntegerOperator(attributeType, comparisonOperator);
                    break;

                case AttributeType.Reference:
                    ValidateReferenceOperator(attributeType, comparisonOperator);
                    break;

                case AttributeType.String:
                    ValidateStringOperator(attributeType, comparisonOperator);
                    break;

                case AttributeType.Text:
                    throw new NotSupportedException("Cannot search on an attribute of type 'Text'");

                case AttributeType.Unknown:
                default:
                    break;
            }
        }

        private static void ValidateIntegerOperator(AttributeType attributeType, ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Contains:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                    ThrowOperatorNotSupported(attributeType, comparisonOperator);
                    break;

                default:
                    break;
            }
        }

        private static void ValidateDateTimeOperator(AttributeType attributeType, ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Contains:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                    ThrowOperatorNotSupported(attributeType, comparisonOperator);
                    break;

                default:
                    break;
            }
        }

        private static void ValidateStringOperator(AttributeType attributeType, ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.GreaterThanOrEquals:
                case ComparisonOperator.LessThan:
                case ComparisonOperator.LessThanOrEquals:
                    ThrowOperatorNotSupported(attributeType, comparisonOperator);
                    break;

                default:
                    break;
            }
        }

        private static void ValidateReferenceOperator(AttributeType attributeType, ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.GreaterThanOrEquals:
                case ComparisonOperator.LessThan:
                case ComparisonOperator.LessThanOrEquals:
                case ComparisonOperator.Contains:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                    ThrowOperatorNotSupported(attributeType, comparisonOperator);
                    break;

                default:
                    break;
            }
        }

        private static void ValidateBooleanOperator(AttributeType attributeType, ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.GreaterThanOrEquals:
                case ComparisonOperator.LessThan:
                case ComparisonOperator.LessThanOrEquals:
                case ComparisonOperator.Contains:
                case ComparisonOperator.StartsWith:
                case ComparisonOperator.EndsWith:
                    ThrowOperatorNotSupported(attributeType, comparisonOperator);
                    break;

                default:
                    break;
            }
        }

        private static void ValidateNegateCombination(ComparisonOperator comparisonOperator, bool negate)
        {
            if (!negate)
            {
                return;
            }

            switch (comparisonOperator)
            {
                case ComparisonOperator.NotEquals:
                case ComparisonOperator.IsPresent:
                case ComparisonOperator.IsNotPresent:
                    throw new InvalidOperationException(string.Format("Cannot negate a query with a {0} operator", comparisonOperator));

                default:
                    break;
            }
        }

        private static void ThrowOperatorNotSupported(AttributeType attributeType, ComparisonOperator comparisonOperator)
        {
            throw new NotSupportedException(string.Format("The operator {0} is not compatible with data type {1}", comparisonOperator, attributeType));
        }
    }
}
