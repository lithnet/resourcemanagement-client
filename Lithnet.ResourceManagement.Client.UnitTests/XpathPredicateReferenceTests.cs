using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathQueryBuilderReferenceTests
    {
        private ResourceManagementClient client = new ResourceManagementClient();

        public XpathQueryBuilderReferenceTests()
        {
            client.DeleteResources(client.GetResources("/" + UnitTestHelper.ObjectTypeUnitTestObjectName));
        }

        // Single-value tests

        [TestMethod]
        public void TestSVReferenceEquals()
        {
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, nonMatchResource.ObjectID);

            object queryValue = nonMatchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceSV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVReferenceNotEquals()
        {
            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, matchResource.ObjectID);

            object queryValue = matchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceSV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVReferenceIsPresent()
        {
            object queryValue = null;

            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, nonMatchResource.ObjectID);

            try
            {
                string expected = string.Format("/{0}[({1} = /*)]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, XPathPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceSV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVReferenceIsNotPresent()
        {
            object queryValue = null;
            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, matchResource.ObjectID);

            try
            {
                string expected = string.Format("/{0}[(not({1} = /*))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, XPathPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceSV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Multivalued tests

        [TestMethod]
        public void TestMVReferenceEquals()
        {
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource2.ObjectID });

            object queryValue = nonMatchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceMV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVReferenceNotEquals()
        {
            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, matchResource.ObjectID);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource2.ObjectID });

            object queryValue = matchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceMV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVReferenceIsPresent()
        {
            object queryValue = null;

            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource.ObjectID });


            try
            {
                string expected = string.Format("/{0}[({1} = /*)]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceMV, XPathPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceMV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVReferenceIsNotPresent()
        {
            object queryValue = null;
            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, matchResource.ObjectID);
            ResourceObject nonMatchResource2 = this.CreateTestResource(UnitTestHelper.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource.ObjectID });

            try
            {
                string expected = string.Format("/{0}[(not({1} = /*))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceMV, XPathPredicate.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceMV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }
        
        // Exception tests

        [TestMethod]
        public void TestSVReferenceGreaterThan()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceSV, XPathOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceGreaterThanOrEquals()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceSV, XPathOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceLessThan()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceSV, XPathOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceLessThanOrEquals()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceSV, XPathOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceContains()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceSV, XPathOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceEndsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceSV, XPathOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceStartsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceSV, XPathOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }


        [TestMethod]
        public void TestMVReferenceGreaterThan()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceMV, XPathOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceGreaterThanOrEquals()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceMV, XPathOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceLessThan()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceMV, XPathOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceLessThanOrEquals()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceMV, XPathOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceContains()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceMV, XPathOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceEndsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceMV, XPathOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceStartsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeReferenceMV, XPathOperator.StartsWith);
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
