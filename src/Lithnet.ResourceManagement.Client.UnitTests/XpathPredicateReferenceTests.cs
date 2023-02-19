using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathQueryBuilderReferenceTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestHelper.DeleteAllTestObjects();
        }

        // Single-value tests

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestSVReferenceEquals(ConnectionMode connectionMode)
        {
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, nonMatchResource.ObjectID);

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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestSVReferenceNotEquals(ConnectionMode connectionMode)
        {
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, matchResource.ObjectID);

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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestSVReferenceIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;

            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, nonMatchResource.ObjectID);

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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestSVReferenceIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, matchResource.ObjectID);

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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestMVReferenceEquals(ConnectionMode connectionMode)
        {
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource2.ObjectID });

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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestMVReferenceNotEquals(ConnectionMode connectionMode)
        {
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, matchResource.ObjectID);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource2.ObjectID });

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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestMVReferenceIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;

            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { nonMatchResource.ObjectID, nonMatchResource.ObjectID });


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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestMVReferenceIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, null);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, matchResource.ObjectID);
            ResourceObject nonMatchResource2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceMV, new List<object>() { matchResource.ObjectID, nonMatchResource.ObjectID });

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

        private void SubmitXpath(object value, string expected, AttributeTypeDefinition attribute, ComparisonOperator xpathOp, GroupOperator queryOp, ConnectionMode connectionMode, params ResourceObject[] matchResources)
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