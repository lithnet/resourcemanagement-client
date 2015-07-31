using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathPredicateBooleanTests
    {
        private ResourceManagementClient client = new ResourceManagementClient();

        public XpathPredicateBooleanTests()
        {
            client.DeleteResources(client.GetResources("/" + UnitTestHelper.ObjectTypeUnitTestObjectName));
        }

        // Single-value tests

        [TestMethod]
        public void TestSVBooleanEquals()
        {
            object queryValue = true;
            object nonMatchValue = false;
            object matchValue = true;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeBooleanSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeBooleanSV, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVBooleanNotEquals()
        {
            object queryValue = true;
            object nonMatchValue = true;
            object matchValue = false;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} != {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeBooleanSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeBooleanSV, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        //[TestMethod]
        public void TesT()
        {
            //string filter = "/Person[descendant-in('Manager', /Person[DisplayName = 'Ryan Newington'])]";
            string filter = "descendants(/Person[DisplayName ='Ryan Newington'], 'Manager')";
            ResourceManagementClient client = new ResourceManagementClient();
            var results = client.GetResources(filter);

        }

        [TestMethod]
        public void TestSVBooleanIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            object matchValue = true;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(({1} = true) or ({1} = false))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeBooleanSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeBooleanSV, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVBooleanIsNotPresent()
        {
            object queryValue = null;
            object nonMatchValue = true;
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not(({1} = true) or ({1} = false)))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeBooleanSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeBooleanSV, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestMethod]
        public void TestSVBooleanGreaterThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeBooleanSV, ComparisonOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanGreaterThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeBooleanSV, ComparisonOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanLessThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeBooleanSV, ComparisonOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanLessThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeBooleanSV, ComparisonOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeBooleanSV, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeBooleanSV, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeBooleanSV, ComparisonOperator.StartsWith);
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