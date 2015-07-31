using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathPredicateDateTimeTests
    {
        private ResourceManagementClient client = new ResourceManagementClient();

        public XpathPredicateDateTimeTests()
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

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeNotEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} != '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeIsNotPresent()
        {
            object queryValue = null;
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeGreaterThan()
        {
            object queryValue = "3000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3100-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.GreaterThan, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeGreaterThanOrEquals()
        {
            object queryValue = "3000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeLessThan()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2100-01-01T00:00:00.000";
            object matchValue = "1900-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.LessThan, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVDateTimeLessThanOrEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2100-01-01T00:00:00.000";
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.LessThanOrEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Multivalued tests

        [TestMethod]
        public void TestMVDateTimeEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000"; ;
            List<string> nonMatchValue = new List<string>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<string> matchValue = new List<string>() { "2300-01-01T00:00:00.000", "2000-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeNotEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<string> nonMatchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };
            List<string> matchValue = new List<string>() { "2200-01-01T00:00:00.000", "2300-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            List<string> matchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeIsNotPresent()
        {
            object queryValue = null;
            List<string> nonMatchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeGreaterThan()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.GreaterThan, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeGreaterThanOrEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeLessThan()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.LessThan, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVDateTimeLessThanOrEquals()
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2000-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.LessThanOrEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestMethod]
        public void TestMVDateTimeContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVDateTimeEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVDateTimeStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeDateTimeMV, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeDateTimeSV, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }


        private void SubmitXpath(object value, string expected, string attributeName, ComparisonOperator xpathOp, GroupOperator queryOp, params ResourceObject[] matchResources)
        {
            XPathQuery predicate = new XPathQuery(attributeName, xpathOp, value);
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