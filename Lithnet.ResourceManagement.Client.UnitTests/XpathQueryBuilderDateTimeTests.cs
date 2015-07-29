using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathQueryBuilderDateTimeTests
    {
        private ResourceManagementClient client = new ResourceManagementClient();

        public XpathQueryBuilderDateTimeTests()
        {
            client.DeleteResources(client.GetResources("/" + UnitTestHelper.ObjectTypeUnitTestObjectName));
        }

        // Single-value tests

        [TestMethod]
        public void TestSVDateTimeEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "3000-01-01T00:00:00.000";
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeNotEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3000-01-01T00:00:00.000";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, XPathFilterPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeIsNotPresent()
        {
            object queryValue = null;
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = null;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, XPathFilterPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeGreaterThan()
        {
            object queryValue = "3000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3100-01-01T00:00:00.000";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, XPathOperator.GreaterThan, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeGreaterThanOrEquals()
        {
            object queryValue = "3000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3000-01-01T00:00:00.000";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, XPathOperator.GreaterThanOrEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeLessThan()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2100-01-01T00:00:00.000";
            object matchValue = "1900-01-01T00:00:00.000";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, XPathOperator.LessThan, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeLessThanOrEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2100-01-01T00:00:00.000";
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, XPathOperator.LessThanOrEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Multivalued tests

        [TestMethod]
        public void TestMVDateTimeEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000"; ;
            List<string> nonMatchValue = new List<string>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<string> matchValue = new List<string>() { "2300-01-01T00:00:00.000", "2000-01-01T00:00:00.000" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeNotEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<string> nonMatchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };
            List<string> matchValue = new List<string>() { "2200-01-01T00:00:00.000", "2300-01-01T00:00:00.000" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            List<string> matchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, XPathFilterPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeIsNotPresent()
        {
            object queryValue = null;
            List<string> nonMatchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };
            object matchValue = null;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, XPathFilterPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeGreaterThan()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, XPathOperator.GreaterThan, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeGreaterThanOrEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, XPathOperator.GreaterThanOrEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeLessThan()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, XPathOperator.LessThan, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeLessThanOrEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2000-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, XPathOperator.LessThanOrEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestMethod]
        public void TestMVDateTimeContains()
        {
            try
            {
                XPathFilterPredicate predicate = new XPathFilterPredicate(UnitTestHelper.AttributeDateTimeMV, XPathOperator.Contains);
                Assert.Fail("The expected exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVDateTimeEndsWith()
        {
            try
            {
                XPathFilterPredicate predicate = new XPathFilterPredicate(UnitTestHelper.AttributeDateTimeMV, XPathOperator.EndsWith);
                Assert.Fail("The expected exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVDateTimeStartsWith()
        {
            try
            {
                XPathFilterPredicate predicate = new XPathFilterPredicate(UnitTestHelper.AttributeDateTimeMV, XPathOperator.StartsWith);
                Assert.Fail("The expected exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeContains()
        {
            try
            {
                XPathFilterPredicate predicate = new XPathFilterPredicate(UnitTestHelper.AttributeDateTimeSV, XPathOperator.Contains);
                Assert.Fail("The expected exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeEndsWith()
        {
            try
            {
                XPathFilterPredicate predicate = new XPathFilterPredicate(UnitTestHelper.AttributeDateTimeSV, XPathOperator.EndsWith);
                Assert.Fail("The expected exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeStartsWith()
        {
            try
            {
                XPathFilterPredicate predicate = new XPathFilterPredicate(UnitTestHelper.AttributeDateTimeSV, XPathOperator.StartsWith);
                Assert.Fail("The expected exception was not thrown");
            }
            catch { }
        }


        private void SubmitXpath(object value, string expected, string attributeName, XPathOperator xpathOp, QueryOperator queryOp, params ResourceObject[] matchResources)
        {
            XPathFilterPredicate predicate = new XPathFilterPredicate(attributeName, xpathOp, value);
            string xpath = XPathFilterBuilder.CreateFilter(UnitTestHelper.ObjectTypeUnitTestObjectName, queryOp, predicate);
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
                Assert.Fail("The query returned results where none were expected");
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

        private ResourceObject CreateTestResource(string attributeName, object value)
        {
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Attributes[attributeName].SetValue(value);
            resource.Save();
            return resource;
        }

        private void CleanupTestResources(params ResourceObject[] resources)
        {
            if (resources == null)
            {
                return;
            }

            foreach (ResourceObject resource in resources)
            {
                if (resource.ModificationType == OperationType.Update)
                {
                    client.DeleteResource(resource);
                }
            }
        }

    }
}
