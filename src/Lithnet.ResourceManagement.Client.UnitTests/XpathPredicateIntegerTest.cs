using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathQueryBuilderIntegerTests
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
        public void TestSVIntegerEquals(ConnectionMode connectionMode)
        {
            object queryValue = 1;
            object nonMatchValue = 2;
            object matchValue = 1;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVIntegerNotEquals(ConnectionMode connectionMode)
        {
            object queryValue = 1;
            object nonMatchValue = 1;
            object matchValue = 2;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVIntegerIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            object nonMatchValue = null;
            object matchValue = 1;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, XPathQuery.MaxLong);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVIntegerIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            object nonMatchValue = 1;
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, XPathQuery.MaxLong);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVIntegerGreaterThan(ConnectionMode connectionMode)
        {
            object queryValue = 10;
            object nonMatchValue = 5;
            object matchValue = 11;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSVDef, ComparisonOperator.GreaterThan, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVIntegerGreaterThanOrEquals(ConnectionMode connectionMode)
        {
            object queryValue = 10;
            object nonMatchValue = 5;
            object matchValue = 10;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSVDef, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVIntegerLessThan(ConnectionMode connectionMode)
        {
            object queryValue = 10;
            object nonMatchValue = 15;
            object matchValue = 9;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSVDef, ComparisonOperator.LessThan, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVIntegerLessThanOrEquals(ConnectionMode connectionMode)
        {
            object queryValue = 10;
            object nonMatchValue = 15;
            object matchValue = 10;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerSVDef, ComparisonOperator.LessThanOrEquals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVIntegerEquals(ConnectionMode connectionMode)
        {
            object queryValue = 1;
            List<long> nonMatchValue = new List<long>() { 2, 3 };
            List<long> matchValue = new List<long>() { 1, 4 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVIntegerNotEquals(ConnectionMode connectionMode)
        {
            object queryValue = 1;
            List<long> nonMatchValue = new List<long>() { 1, 3 };
            List<long> matchValue = new List<long>() { 3, 4 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVIntegerIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            object nonMatchValue = null;
            List<long> matchValue = new List<long>() { 1, 2 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, XPathQuery.MaxLong);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVIntegerIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            List<long> nonMatchValue = new List<long>() { 1, 2 };
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, XPathQuery.MaxLong);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVIntegerGreaterThan(ConnectionMode connectionMode)
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 9, 8 };
            List<object> matchValue = new List<object>() { 9, 11 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMVDef, ComparisonOperator.GreaterThan, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVIntegerGreaterThanOrEquals(ConnectionMode connectionMode)
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 9, 8 };
            List<object> matchValue = new List<object>() { 9, 10 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMVDef, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVIntegerLessThan(ConnectionMode connectionMode)
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 15, 20 };
            List<object> matchValue = new List<object>() { 9, 20 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMVDef, ComparisonOperator.LessThan, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVIntegerLessThanOrEquals(ConnectionMode connectionMode)
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 15, 20 };
            List<object> matchValue = new List<object>() { 10, 20 };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeIntegerMVDef, ComparisonOperator.LessThanOrEquals, GroupOperator.And, connectionMode, matchResource);
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
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerMVDef, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVIntegerEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerMVDef, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVIntegerStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerMVDef, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerSVDef, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerSVDef, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeIntegerSVDef, ComparisonOperator.StartsWith);
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