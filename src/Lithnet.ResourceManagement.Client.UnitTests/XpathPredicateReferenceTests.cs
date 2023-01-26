using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathQueryBuilderReferenceTests
    {
        private ResourceManagementClient client = UnitTestHelper.ServiceProvider.GetRequiredService<ResourceManagementClient>();

        public XpathQueryBuilderReferenceTests()
        {
            client.DeleteResources(client.GetResources("/" + Constants.UnitTestObjectTypeName));
        }

        // Single-value tests

        [TestMethod]
        public void TestSVReferenceEquals()
        {
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, nonMatchResource.ObjectID);

            object queryValue = nonMatchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceSVDef, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVReferenceNotEquals()
        {
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, matchResource.ObjectID);

            object queryValue = matchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceSVDef, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVReferenceIsPresent()
        {
            object queryValue = null;

            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, nonMatchResource.ObjectID);

            try
            {
                string expected = string.Format("/{0}[({1} = /*)]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceSVDef, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVReferenceIsNotPresent()
        {
            object queryValue = null;
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, matchResource.ObjectID);

            try
            {
                string expected = string.Format("/{0}[(not({1} = /*))]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceSVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Multivalued tests

        [TestMethod]
        public void TestMVReferenceEquals()
        {
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource2.ObjectID });

            object queryValue = nonMatchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceMVDef, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVReferenceNotEquals()
        {
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, matchResource.ObjectID);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource2.ObjectID });

            object queryValue = matchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceMVDef, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVReferenceIsPresent()
        {
            object queryValue = null;

            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource.ObjectID });


            try
            {
                string expected = string.Format("/{0}[({1} = /*)]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceMVDef, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVReferenceIsNotPresent()
        {
            object queryValue = null;
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, matchResource.ObjectID);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource.ObjectID });

            try
            {
                string expected = string.Format("/{0}[(not({1} = /*))]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeReferenceMVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestMethod]
        public void TestSVReferenceGreaterThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceSVDef, ComparisonOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceGreaterThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceSVDef, ComparisonOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceLessThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceSVDef, ComparisonOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceLessThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceSVDef, ComparisonOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceSVDef, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceSVDef, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceSVDef, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }


        [TestMethod]
        public void TestMVReferenceGreaterThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceMVDef, ComparisonOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceGreaterThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceMVDef, ComparisonOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceLessThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceMVDef, ComparisonOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceLessThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceMVDef, ComparisonOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceMVDef, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceMVDef, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeReferenceMVDef, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        private void SubmitXpath(object value, string expected, AttributeTypeDefinition attribute, ComparisonOperator xpathOp, GroupOperator queryOp, params ResourceObject[] matchResources)
        {
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