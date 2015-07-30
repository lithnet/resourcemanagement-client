using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeBooleanSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeBooleanSV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVBooleanNotEquals()
        {
            object queryValue = true;
            object nonMatchValue = true;
            object matchValue = false;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} != {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeBooleanSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeBooleanSV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
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

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(({1} = true) or ({1} = false))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeBooleanSV, XPathPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeBooleanSV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVBooleanIsNotPresent()
        {
            object queryValue = null;
            object nonMatchValue = true;
            object matchValue = null;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not(({1} = true) or ({1} = false)))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeBooleanSV, XPathPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeBooleanSV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestMethod]
        public void TestSVBooleanGreaterThan()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeBooleanSV, XPathOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanGreaterThanOrEquals()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeBooleanSV, XPathOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanLessThan()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeBooleanSV, XPathOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanLessThanOrEquals()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeBooleanSV, XPathOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanContains()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeBooleanSV, XPathOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanEndsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeBooleanSV, XPathOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanStartsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeBooleanSV, XPathOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }


        private void SubmitXpath(object value, string expected, string attributeName, XPathOperator xpathOp, QueryOperator queryOp, params ResourceObject[] matchResources)
        {
            XPathPredicate predicate = new XPathPredicate(attributeName, xpathOp, value);
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
