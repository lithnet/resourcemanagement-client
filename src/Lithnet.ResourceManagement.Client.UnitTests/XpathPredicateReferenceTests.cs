using System.Collections.Generic;
using System;
using System.Linq;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{

    public class XpathQueryBuilderReferenceTests
    {
        [SetUp]
        public void TestInitialize()
        {
            UnitTestHelper.DeleteAllTestObjects();
        }

        // Single-value tests
        [TestCaseSource(typeof(ConnectionModeSources))]
        public void TestSVReferenceEquals(ConnectionMode connectionMode)
        {
            IResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            IResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, nonMatchResource.ObjectID);

            object queryValue = nonMatchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceSVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void TestSVReferenceNotEquals(ConnectionMode connectionMode)
        {
            IResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            IResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, matchResource.ObjectID);

            object queryValue = matchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceSVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void TestSVReferenceIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;

            IResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            IResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, nonMatchResource.ObjectID);

            try
            {
                string expected = string.Format("/{0}[({1} = /*)]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceSVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void TestSVReferenceIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            IResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            IResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, matchResource.ObjectID);

            try
            {
                string expected = string.Format("/{0}[(not({1} = /*))]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceSVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Multivalued tests
        [TestCaseSource(typeof(ConnectionModeSources))]
        public void TestMVReferenceEquals(ConnectionMode connectionMode)
        {
            IResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            IResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            IResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource2.ObjectID });

            object queryValue = nonMatchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceMVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void TestMVReferenceNotEquals(ConnectionMode connectionMode)
        {
            IResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            IResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, matchResource.ObjectID);
            IResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource2.ObjectID });

            object queryValue = matchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceMVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void TestMVReferenceIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;

            IResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            IResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);

            IResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource.ObjectID });


            try
            {
                string expected = string.Format("/{0}[({1} = /*)]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceMVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void TestMVReferenceIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            IResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            IResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, matchResource.ObjectID);
            IResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource.ObjectID });

            try
            {
                string expected = string.Format("/{0}[(not({1} = /*))]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceMVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestCase(false, ComparisonOperator.GreaterThan)]
        [TestCase(false, ComparisonOperator.GreaterThanOrEquals)]
        [TestCase(false, ComparisonOperator.LessThan)]
        [TestCase(false, ComparisonOperator.LessThanOrEquals)]
        [TestCase(false, ComparisonOperator.Contains)]
        [TestCase(false, ComparisonOperator.EndsWith)]
        [TestCase(false, ComparisonOperator.StartsWith)]
        [TestCase(true, ComparisonOperator.GreaterThan)]
        [TestCase(true, ComparisonOperator.GreaterThanOrEquals)]
        [TestCase(true, ComparisonOperator.LessThan)]
        [TestCase(true, ComparisonOperator.LessThanOrEquals)]
        [TestCase(true, ComparisonOperator.Contains)]
        [TestCase(true, ComparisonOperator.EndsWith)]
        [TestCase(true, ComparisonOperator.StartsWith)]
        public void TestReferenceUnsupportedOperator(bool multivalued, ComparisonOperator comparisonOperator)
        {
            AttributeTypeDefinition attribute = multivalued
                ? Constants.AttributeReferenceMVDef
                : Constants.AttributeReferenceSVDef;

            Assert.Throws<NotSupportedException>(() => new XPathQuery(attribute, comparisonOperator, Guid.Empty));
        }

        private void SubmitXpath(object value, string expected, AttributeTypeDefinition attribute, ComparisonOperator xpathOp, GroupOperator queryOp, ConnectionMode connectionMode, params IResourceObject[] matchResources)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            XPathQuery predicate = new XPathQuery(attribute, xpathOp, value);
            string xpath = XPathFilterBuilder.CreateFilter(Constants.UnitTestObjectTypeName, queryOp, predicate);
            Assert.AreEqual(expected, xpath);

            ISearchResultCollection results = client.GetResources(xpath);

            if (results.Count == 0)
            {
                if (matchResources != null && matchResources.Length > 0)
                {
                    Assert.Fail("The query returned no results");
                }
            }

            if (matchResources == null || matchResources.Length == 0)
            {
                Assert.Fail("The query returned results where none were expectedXpath");
            }

            if (results.Count != matchResources.Length)
            {
                Assert.Fail("The query returned an unexpected number of results");
            }

            if (!results.All(t => matchResources.Any(u => u.ObjectID == t.ObjectID)))
            {
                Assert.Fail("The query did not return the correct results");
            }
        }
    }
}
