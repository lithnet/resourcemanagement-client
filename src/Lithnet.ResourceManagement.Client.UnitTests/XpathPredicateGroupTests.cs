using System.Linq;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{

    public class XpathPredicateGroupTests
    {
        [SetUp]
        public void TestInitialize()
        {
            UnitTestHelper.DeleteAllTestObjects();
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void XpathPredicateGroupSingleValueTest(ConnectionMode connectionMode)
        {
            string testValue1 = "test1";
            string nonMatchValue = "test3";
            XPathQuery predicate1 = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.Equals, testValue1);
            XPathQueryGroup group = new XPathQueryGroup(GroupOperator.And, predicate1);

            string expected = string.Format("({0} = '{1}')", Constants.AttributeStringSV, testValue1);

            ResourceObject matchObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, testValue1);
            ResourceObject nonMatchObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                this.SubmitXpath(group, expected, connectionMode, matchObject);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchObject, nonMatchObject);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void XpathPredicateGroupAndTest(ConnectionMode connectionMode)
        {
            string testValue1 = "test1";
            string testValue2 = "test2";
            string nonMatchValue = "test3";
            XPathQuery predicate1 = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.Equals, testValue1);
            XPathQuery predicate2 = new XPathQuery(Constants.AttributeStringMVDef, ComparisonOperator.Equals, testValue2);
            XPathQueryGroup group = new XPathQueryGroup(GroupOperator.And, predicate1, predicate2);

            string expected = string.Format("(({0} = '{1}') and ({2} = '{3}'))", Constants.AttributeStringSV, testValue1, Constants.AttributeStringMV, testValue2);

            ResourceObject matchObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, testValue1);
            ResourceObject nonMatchObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);
            matchObject.Attributes[Constants.AttributeStringMV].SetValue(testValue2);
            matchObject.Save();

            try
            {
                this.SubmitXpath(group, expected, connectionMode, matchObject);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchObject, nonMatchObject);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void XpathPredicateGroupOrTest(ConnectionMode connectionMode)
        {
            string testValue1 = "test1";
            string testValue2 = "test2";
            string nonMatchValue = "test3";
            XPathQuery predicate1 = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.Equals, testValue1);
            XPathQuery predicate2 = new XPathQuery(Constants.AttributeStringMVDef, ComparisonOperator.Equals, testValue2);
            XPathQueryGroup group = new XPathQueryGroup(GroupOperator.Or, predicate1, predicate2);

            string expected = string.Format("(({0} = '{1}') or ({2} = '{3}'))", Constants.AttributeStringSV, testValue1, Constants.AttributeStringMV, testValue2);

            ResourceObject matchObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, testValue1);
            ResourceObject nonMatchObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                this.SubmitXpath(group, expected, connectionMode, matchObject);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchObject, nonMatchObject);
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void XpathPredicateGroupNestedTest(ConnectionMode connectionMode)
        {
            string testValue1 = "test1";
            string testValue2 = "test2";
            long testValue3 = 55L;
            string nonMatchValue = "test3";
            XPathQuery predicate1 = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.Equals, testValue1);
            XPathQuery predicate2 = new XPathQuery(Constants.AttributeStringMVDef, ComparisonOperator.Equals, testValue2);
            XPathQuery predicate3 = new XPathQuery(Constants.AttributeIntegerSVDef, ComparisonOperator.Equals, testValue3);
            XPathQueryGroup childGroup = new XPathQueryGroup(GroupOperator.Or, predicate1, predicate2);
            XPathQueryGroup group = new XPathQueryGroup(GroupOperator.And, predicate3, childGroup);

            string expected = string.Format("(({4} = {5}) and (({0} = '{1}') or ({2} = '{3}')))", Constants.AttributeStringSV, testValue1, Constants.AttributeStringMV, testValue2, Constants.AttributeIntegerSV, testValue3);

            ResourceObject matchObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, testValue1);
            ResourceObject nonMatchObject = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);
            matchObject.Attributes[Constants.AttributeIntegerSV].SetValue(testValue3);
            matchObject.Save();

            try
            {
                this.SubmitXpath(group, expected, connectionMode, matchObject);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchObject, nonMatchObject);
            }
        }

        private void SubmitXpath(XPathQueryGroup group, string expectedXpath, ConnectionMode connectionMode, params ResourceObject[] matchResources)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            Assert.AreEqual(expectedXpath, group.ToString());

            XPathExpression expression = new XPathExpression(Constants.UnitTestObjectTypeName, group);

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
                Assert.Fail("The query returned an unexpected number of results");
            }

            if (!results.All(t => matchResources.Any(u => u.ObjectID == t.ObjectID)))
            {
                Assert.Fail("The query did not return the correct results");
            }
        }
    }
}