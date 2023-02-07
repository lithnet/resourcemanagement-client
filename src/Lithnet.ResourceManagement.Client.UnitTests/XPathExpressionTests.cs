using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XPathExpressionTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestHelper.DeleteAllTestObjects();
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void ThrowOnInvalidObjectTypeName(ConnectionMode connectionMode)
        {
            try
            {
                string testValue1 = "test1";
                XPathQuery predicate1 = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.Equals, testValue1);
                XPathExpression childExpression = new XPathExpression("legalName", predicate1);
                childExpression = new XPathExpression("also:legalName", predicate1);
                childExpression = new XPathExpression("%%%%%", predicate1);

                Assert.Fail("The expected exception was not thrown");
            }
            catch { }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void XpathExpressionNestedTest(ConnectionMode connectionMode)
        {
            string testValue1 = "test1";
            XPathQuery predicate1 = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.Equals, testValue1);
            XPathExpression childExpression = new XPathExpression(Constants.UnitTestObjectTypeName, predicate1);
            XPathQuery predicate2 = new XPathQuery(Constants.AttributeReferenceSVDef, ComparisonOperator.Equals, childExpression);
            XPathExpression expression = new XPathExpression(Constants.UnitTestObjectTypeName, predicate2);

            string expected = string.Format("/{0}[({1} = /{0}[({2} = '{3}')])]", Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, Constants.AttributeStringSV, testValue1);

            ResourceObject filterTargetObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, testValue1);
            ResourceObject childObject1 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, filterTargetObject.ObjectID);
            ResourceObject childObject2 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, filterTargetObject.ObjectID);
            ResourceObject childObject3 = UnitTestHelper.CreateTestResource(Constants.AttributeReferenceSV, filterTargetObject.ObjectID);

            try
            {
                this.SubmitXpath(expression, expected, connectionMode, childObject1, childObject2, childObject3);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(filterTargetObject, childObject1, childObject2, childObject3);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void XpathExpressionDereferencedTest(ConnectionMode connectionMode)
        {
            string testValue1 = "test1";
            XPathQuery predicate1 = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.Equals, testValue1);
            XPathDereferencedExpression expression = new XPathDereferencedExpression(Constants.UnitTestObjectTypeName, Constants.AttributeReferenceSV, predicate1);

            string expected = string.Format("/{0}[({1} = '{2}')]/{3}", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, testValue1, Constants.AttributeReferenceSV);

            ResourceObject parentObject1 = UnitTestHelper.CreateTestResource();
            ResourceObject filterTargetObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, testValue1, Constants.AttributeReferenceSV, parentObject1);

            try
            {
                this.SubmitXpath(expression, expected, connectionMode, parentObject1);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(filterTargetObject, parentObject1);
            }
        }

        private void SubmitXpath(XPathExpression expression, string expectedXpath, ConnectionMode connectionMode, params ResourceObject[] matchResources)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            Assert.AreEqual(expectedXpath, expression.ToString());

            ISearchResultCollection results = client.GetResources(expression.ToString());

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
                Assert.Fail("The query returned an unexpected number of results. Expected {0}, Actual {1}", matchResources.Length, results.Count);
            }

            if (!results.All(t => matchResources.Any(u => u.ObjectID == t.ObjectID)))
            {
                Assert.Fail("The query did not return the correct results");
            }
        }
    }
}
