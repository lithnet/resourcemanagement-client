using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathQueryBuilderIntegerTests
    {
        private ResourceManagementClient client = UnitTestHelper.ServiceProvider.GetRequiredService<ResourceManagementClient>();

        public XpathQueryBuilderIntegerTests()
        {
            client.DeleteResources(client.GetResources("/" + Constants.UnitTestObjectTypeName));
        }

        // Single-value tests

        [TestMethod]
        public void TestSVIntegerEquals()
        {
            object queryValue = 1;
            object nonMatchValue = 2;
            object matchValue = 1;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSV, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerNotEquals()
        {
            object queryValue = 1;
            object nonMatchValue = 1;
            object matchValue = 2;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSV, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            object matchValue = 1;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, XPathQuery.MaxLong);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSV, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerIsNotPresent()
        {
            object queryValue = null;
            object nonMatchValue = 1;
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, XPathQuery.MaxLong);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSV, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerGreaterThan()
        {
            object queryValue = 10;
            object nonMatchValue = 5;
            object matchValue = 11;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSV, ComparisonOperator.GreaterThan, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerGreaterThanOrEquals()
        {
            object queryValue = 10;
            object nonMatchValue = 5;
            object matchValue = 10;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSV, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerLessThan()
        {
            object queryValue = 10;
            object nonMatchValue = 15;
            object matchValue = 9;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSV, ComparisonOperator.LessThan, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerLessThanOrEquals()
        {
            object queryValue = 10;
            object nonMatchValue = 15;
            object matchValue = 10;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSV, ComparisonOperator.LessThanOrEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Multivalued tests

        [TestMethod]
        public void TestMVIntegerEquals()
        {
            object queryValue = 1;
            List<long> nonMatchValue = new List<long>() { 2, 3 };
            List<long> matchValue = new List<long>() { 1, 4 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMV, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerNotEquals()
        {
            object queryValue = 1;
            List<long> nonMatchValue = new List<long>() { 1, 3 };
            List<long> matchValue = new List<long>() { 3, 4 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMV, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            List<long> matchValue = new List<long>() { 1, 2 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, XPathQuery.MaxLong);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMV, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerIsNotPresent()
        {
            object queryValue = null;
            List<long> nonMatchValue = new List<long>() { 1, 2 };
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, XPathQuery.MaxLong);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMV, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerGreaterThan()
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 9, 8 };
            List<object> matchValue = new List<object>() { 9, 11 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMV, ComparisonOperator.GreaterThan, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerGreaterThanOrEquals()
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 9, 8 };
            List<object> matchValue = new List<object>() { 9, 10 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMV, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerLessThan()
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 15, 20 };
            List<object> matchValue = new List<object>() { 9, 20 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMV, ComparisonOperator.LessThan, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerLessThanOrEquals()
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 15, 20 };
            List<object> matchValue = new List<object>() { 10, 20 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMV, ComparisonOperator.LessThanOrEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestMethod]
        public void TestMVIntegerContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerMV, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVIntegerEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerMV, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVIntegerStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerMV, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerSV, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerSV, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerSV, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        private void SubmitXpath(object value, string expected, string attributeName, ComparisonOperator xpathOp, GroupOperator queryOp, params ResourceObject[] matchResources)
        {
            XPathQuery predicate = new XPathQuery(attributeName, xpathOp, value);
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