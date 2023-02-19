using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathPredicateStringTests
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
        public void TestSVStringEquals(ConnectionMode connectionMode)
        {
            string queryValue = "user0001";
            string nonMatchValue = "user0002";
            string matchValue = "user0001";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVStringEqualsWithSingleQuote(ConnectionMode connectionMode)
        {
            string queryValue = "user'0001";
            string nonMatchValue = "user0002";
            string matchValue = "user'0001";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = \"{2}\")]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVStringEqualsWithDoubleQuote(ConnectionMode connectionMode)
        {
            string queryValue = "user\"0001";
            string nonMatchValue = "user0002";
            string matchValue = "user\"0001";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVStringEqualsWithSingleAndDoubleQuote(ConnectionMode connectionMode)
        {
            string queryValue = "user\"'0001";
            string nonMatchValue = "user0002";
            string matchValue = "user\"'0001";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
            }
            catch (ArgumentException)
            {
                return;
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }

            Assert.Fail("The expected exception was not thrown");
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestSVStringNotEquals(ConnectionMode connectionMode)
        {
            string queryValue = "user0001";
            string nonMatchValue = "user0001";
            string matchValue = "user0002";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVStringContains(ConnectionMode connectionMode)
        {
            /*
              FROM https://msdn.microsoft.com/en-us/library/windows/desktop/ee652287(v=vs.100).aspx
            
                Given the following string: "The quick brown fox," and the contains() query on the string "u", the expectedXpath result is 
                that nothing is returned, since the letter "u" only appears in the middle of a string, and not immediately after a word-breaker. 
                If we ran the contains() query on the string "qu", however, we would get a match, since the substring "qu" appears immediately 
                after a word-breaking character.
            */

            string queryValue = "abc";
            string nonMatchValue = "user def";
            string matchValue = "user abc";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(contains({1}, '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.Contains, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVStringEndsWith(ConnectionMode connectionMode)
        {
            string queryValue = "0001";
            string nonMatchValue = "user0002";
            string matchValue = "user0001";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(ends-with({1}, '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.EndsWith, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVStringStartsWith(ConnectionMode connectionMode)
        {
            string queryValue = "y";
            string nonMatchValue = "xuser0002";
            string matchValue = "yuser0001";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(starts-with({1}, '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.StartsWith, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVStringIsPresent(ConnectionMode connectionMode)
        {
            string queryValue = null;
            string nonMatchValue = null;
            string matchValue = "user0001";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(starts-with({1}, '%'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
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
        public void TestSVStringIsNotPresent(ConnectionMode connectionMode)
        {
            string queryValue = null;
            string nonMatchValue = "user0001";
            string matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not(starts-with({1}, '%')))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringSVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVStringEquals(ConnectionMode connectionMode)
        {
            string queryValue = "user0001";
            List<string> nonMatchValue = new List<string>() { "user0003", "user0004" };
            List<string> matchValue = new List<string>() { "user0001", "user0002" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringMVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVStringNotEquals(ConnectionMode connectionMode)
        {
            string queryValue = "user0001";
            List<string> nonMatchValue = new List<string>() { "user0001", "user0002" };
            List<string> matchValue = new List<string>() { "user0003", "user0004" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringMVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVStringContains(ConnectionMode connectionMode)
        {
            /*
              FROM https://msdn.microsoft.com/en-us/library/windows/desktop/ee652287(v=vs.100).aspx
            
                Given the following string: "The quick brown fox," and the contains() query on the string "u", the expectedXpath result is 
                that nothing is returned, since the letter "u" only appears in the middle of a string, and not immediately after a wordbreaker. 
                If we ran the contains() query on the string "qu", however, we would get a match, since the substring "qu" appears immediately 
                after a wordbreaking character.
            */

            string queryValue = "abc";
            List<string> nonMatchValue = new List<string>() { "123 def", "456 def" };
            List<string> matchValue = new List<string>() { "sdf abc", "1011 ghi" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(contains({1}, '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringMVDef, ComparisonOperator.Contains, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVStringEndsWith(ConnectionMode connectionMode)
        {
            string queryValue = "0001";
            List<string> nonMatchValue = new List<string>() { "user0004", "user0003" };
            List<string> matchValue = new List<string>() { "user0002", "user0001" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(ends-with({1}, '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringMVDef, ComparisonOperator.EndsWith, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVStringStartsWith(ConnectionMode connectionMode)
        {
            string queryValue = "y";
            List<string> nonMatchValue = new List<string>() { "xuser0004", "xuser0003" };
            List<string> matchValue = new List<string>() { "yuser0002", "yuser0001" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(starts-with({1}, '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringMVDef, ComparisonOperator.StartsWith, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVStringIsPresent(ConnectionMode connectionMode)
        {
            string queryValue = null;
            string nonMatchValue = null;
            List<string> matchValue = new List<string>() { "user0001", "user0002" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(starts-with({1}, '%'))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringMVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
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
        public void TestMVStringIsNotPresent(ConnectionMode connectionMode)
        {
            string queryValue = null;
            List<string> nonMatchValue = new List<string>() { "user0001", "user0002" };
            string matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not(starts-with({1}, '%')))]", Constants.UnitTestObjectTypeName, Constants.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeStringMVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestMethod]
        public void TestStringGreaterThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestStringGreaterThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestStringLessThan()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestStringLessThanOrEquals()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeStringSVDef, ComparisonOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        private void SubmitXpath(string value, string expected, AttributeTypeDefinition attribute, ComparisonOperator xpathOp, GroupOperator queryOp, ConnectionMode connectionMode, params ResourceObject[] matchResources)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            XPathQuery predicate = new XPathQuery(attribute, xpathOp, value);
            string xpath = XPathFilterBuilder.CreateFilter(Constants.UnitTestObjectTypeName, queryOp, predicate);
            Assert.AreEqual(expected, xpath);

            if (xpathOp == ComparisonOperator.Contains)
            {
                Thread.Sleep(8000);
            }

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