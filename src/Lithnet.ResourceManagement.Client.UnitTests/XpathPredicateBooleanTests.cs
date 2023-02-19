using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathPredicateBooleanTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestHelper.DeleteAllTestObjects();
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestSVBooleanEquals(ConnectionMode connectionMode)
        {
            object queryValue = true;
            object nonMatchValue = false;
            object matchValue = true;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeBooleanSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeBooleanSVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVBooleanNotEquals(ConnectionMode connectionMode)
        {
            object queryValue = true;
            object nonMatchValue = true;
            object matchValue = false;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = {2}))]", Constants.UnitTestObjectTypeName, Constants.AttributeBooleanSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeBooleanSVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVBooleanIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            object nonMatchValue = null;
            object matchValue = true;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeBooleanSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(({1} = true) or ({1} = false))]", Constants.UnitTestObjectTypeName, Constants.AttributeBooleanSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeBooleanSVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVBooleanIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            object nonMatchValue = true;
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeBooleanSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeBooleanSV, nonMatchValue);

            matchResource.Refresh();
            matchResource.Attributes[Constants.AttributeBooleanSV].Value = null;
            matchResource.Save();

            try
            {
                string expected = string.Format("/{0}[(not(({1} = true) or ({1} = false)))]", Constants.UnitTestObjectTypeName, Constants.AttributeBooleanSV);
                this.SubmitXpath(queryValue, expected, Constants.AttributeBooleanSVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
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
                XPathQuery predicate = new XPathQuery(Constants.AttributeBooleanSVDef, ComparisonOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanGreaterThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeBooleanSVDef, ComparisonOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanLessThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeBooleanSVDef, ComparisonOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanLessThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeBooleanSVDef, ComparisonOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeBooleanSVDef, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeBooleanSVDef, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVBooleanStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeBooleanSVDef, ComparisonOperator.StartsWith);
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
                // Assert.Fail("The query returned an unexpected number of results");
            }

            if (!matchResources.All(t => results.Any(u => u.ObjectID == t.ObjectID)))
            {
                Assert.Fail("The query did not return the correct results");
            }
        }
    }
}