using System;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.DiscoveryTests
{
    public class XPathQueryValidatorTests
    {
        [Test]
        public void NullAttributeDefinitionThrowsArgumentNullException()
        {
            ArgumentNullException validatorException = Assert.Throws<ArgumentNullException>(() =>
                XPathQueryValidator.Validate((AttributeTypeDefinition)null, ComparisonOperator.Equals, "value", false));
            ArgumentNullException constructorException = Assert.Throws<ArgumentNullException>(() =>
                new XPathQuery((AttributeTypeDefinition)null, ComparisonOperator.Equals, "value", false));

            Assert.AreEqual("attribute", validatorException.ParamName);
            Assert.AreEqual("attribute", constructorException.ParamName);
        }

        [Test]
        public void NullAttributeNameThrowsArgumentNullException()
        {
            ArgumentNullException validatorException = Assert.Throws<ArgumentNullException>(() =>
                XPathQueryValidator.Validate(null, AttributeType.String, ComparisonOperator.Equals, "value", false));
            ArgumentNullException constructorException = Assert.Throws<ArgumentNullException>(() =>
                new XPathQuery(null, AttributeType.String, ComparisonOperator.Equals, "value", false));

            Assert.AreEqual("attributeName", validatorException.ParamName);
            Assert.AreEqual("attributeName", constructorException.ParamName);
        }

        [TestCase("")]
        [TestCase(" ")]
        public void BlankAttributeNameThrowsArgumentException(string attributeName)
        {
            ArgumentException validatorException = Assert.Throws<ArgumentException>(() =>
                XPathQueryValidator.Validate(attributeName, AttributeType.String, ComparisonOperator.Equals, "value", false));
            ArgumentException constructorException = Assert.Throws<ArgumentException>(() =>
                new XPathQuery(attributeName, AttributeType.String, ComparisonOperator.Equals, "value", false));

            Assert.AreEqual("attributeName", validatorException.ParamName);
            Assert.AreEqual("attributeName", constructorException.ParamName);
        }

        [TestCase(ComparisonOperator.Equals)]
        [TestCase(ComparisonOperator.NotEquals)]
        [TestCase(ComparisonOperator.GreaterThan)]
        [TestCase(ComparisonOperator.GreaterThanOrEquals)]
        [TestCase(ComparisonOperator.LessThan)]
        [TestCase(ComparisonOperator.LessThanOrEquals)]
        [TestCase(ComparisonOperator.Contains)]
        [TestCase(ComparisonOperator.StartsWith)]
        [TestCase(ComparisonOperator.EndsWith)]
        public void MissingRequiredValueThrowsArgumentNullException(ComparisonOperator comparisonOperator)
        {
            ArgumentNullException validatorException = Assert.Throws<ArgumentNullException>(() =>
                XPathQueryValidator.Validate("Attribute", AttributeType.Unknown, comparisonOperator, null, false));
            ArgumentNullException constructorException = Assert.Throws<ArgumentNullException>(() =>
                new XPathQuery("Attribute", AttributeType.Unknown, comparisonOperator, null, false));

            Assert.AreEqual("value", validatorException.ParamName);
            Assert.AreEqual("value", constructorException.ParamName);
        }

        [TestCase(ComparisonOperator.IsPresent)]
        [TestCase(ComparisonOperator.IsNotPresent)]
        public void PresenceOperatorAllowsNullValue(ComparisonOperator comparisonOperator)
        {
            Assert.DoesNotThrow(() =>
                XPathQueryValidator.Validate("Attribute", AttributeType.String, comparisonOperator, null, false));
            Assert.DoesNotThrow(() =>
                new XPathQuery("Attribute", AttributeType.String, comparisonOperator, null, false));
        }

        [TestCase(AttributeType.Binary, ComparisonOperator.Equals)]
        [TestCase(AttributeType.Boolean, ComparisonOperator.Contains)]
        [TestCase(AttributeType.DateTime, ComparisonOperator.Contains)]
        [TestCase(AttributeType.Integer, ComparisonOperator.Contains)]
        [TestCase(AttributeType.Reference, ComparisonOperator.Contains)]
        [TestCase(AttributeType.String, ComparisonOperator.GreaterThan)]
        [TestCase(AttributeType.Text, ComparisonOperator.Equals)]
        public void UnsupportedTypeOperatorCombinationThrowsNotSupportedException(
            AttributeType attributeType,
            ComparisonOperator comparisonOperator)
        {
            Assert.Throws<NotSupportedException>(() =>
                XPathQueryValidator.Validate("Attribute", attributeType, comparisonOperator, "value", false));
            Assert.Throws<NotSupportedException>(() =>
                new XPathQuery("Attribute", attributeType, comparisonOperator, "value", false));
        }

        [TestCase(ComparisonOperator.NotEquals)]
        [TestCase(ComparisonOperator.IsPresent)]
        [TestCase(ComparisonOperator.IsNotPresent)]
        public void UnsupportedNegationThrowsInvalidOperationException(ComparisonOperator comparisonOperator)
        {
            object value = comparisonOperator == ComparisonOperator.NotEquals ? "value" : null;

            Assert.Throws<InvalidOperationException>(() =>
                XPathQueryValidator.Validate("Attribute", AttributeType.String, comparisonOperator, value, true));
            Assert.Throws<InvalidOperationException>(() =>
                new XPathQuery("Attribute", AttributeType.String, comparisonOperator, value, true));
        }

        [Test]
        public void ValidQueryPassesValidatorAndConstructor()
        {
            Assert.DoesNotThrow(() =>
                XPathQueryValidator.Validate("Attribute", AttributeType.String, ComparisonOperator.Equals, "value", false));
            Assert.DoesNotThrow(() =>
                new XPathQuery("Attribute", AttributeType.String, ComparisonOperator.Equals, "value", false));
        }
    }
}
