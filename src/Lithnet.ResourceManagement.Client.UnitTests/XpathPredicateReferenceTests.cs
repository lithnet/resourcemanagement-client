using System.Collections.Generic;
using System.Linq;
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
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceSV, nonMatchResource.ObjectID);

            object queryValue = nonMatchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceSV, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVReferenceNotEquals()
        {
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceSV, matchResource.ObjectID);

            object queryValue = matchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceSV, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
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

            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceSV, nonMatchResource.ObjectID);

            try
            {
                string expected = string.Format("/{0}[({1} = /*)]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceSV, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
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
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceSV, matchResource.ObjectID);

            try
            {
                string expected = string.Format("/{0}[(not({1} = /*))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceSV, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
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
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource2.ObjectID });

            object queryValue = nonMatchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceMV, ComparisonOperator.Equals, GroupOperator.And, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVReferenceNotEquals()
        {
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, matchResource.ObjectID);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource2.ObjectID });

            object queryValue = matchResource.ObjectID.Value;

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceMV, ComparisonOperator.NotEquals, GroupOperator.And, matchResource);
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

            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource.ObjectID });


            try
            {
                string expected = string.Format("/{0}[({1} = /*)]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceMV, ComparisonOperator.IsPresent, GroupOperator.And, matchResource);
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
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, matchResource.ObjectID);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource.ObjectID });

            try
            {
                string expected = string.Format("/{0}[(not({1} = /*))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeReferenceMV, ComparisonOperator.IsNotPresent, GroupOperator.And, matchResource);
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
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceSV, ComparisonOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceGreaterThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceSV, ComparisonOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceLessThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceSV, ComparisonOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceLessThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceSV, ComparisonOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceSV, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceSV, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVReferenceStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceSV, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }


        [TestMethod]
        public void TestMVReferenceGreaterThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceMV, ComparisonOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceGreaterThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceMV, ComparisonOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceLessThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceMV, ComparisonOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceLessThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceMV, ComparisonOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceMV, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceMV, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVReferenceStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(UnitTestHelper.AttributeReferenceMV, ComparisonOperator.StartsWith);
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