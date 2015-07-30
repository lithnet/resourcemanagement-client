using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathPredicateStringTests
    {
        private ResourceManagementClient client = new ResourceManagementClient();

        public XpathPredicateStringTests()
        {
            client.DeleteResources(client.GetResources("/" + UnitTestHelper.ObjectTypeUnitTestObjectName));
        }

        // Single-value tests

        [TestMethod]
        public void TestSVStringEquals()
        {
            string queryValue = "user0001";
            string nonMatchValue = "user0002";
            string matchValue = "user0001";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringSV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVStringNotEquals()
        {
            string queryValue = "user0001";
            string nonMatchValue = "user0001";
            string matchValue = "user0002";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} != '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringSV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVStringContains()
        {
            /*
              FROM https://msdn.microsoft.com/en-us/library/windows/desktop/ee652287(v=vs.100).aspx
            
                Given the following string: "The quick brown fox," and the contains() query on the string "u", the expectedXpath result is 
                that nothing is returned, since the letter "u" only appears in the middle of a string, and not immediately after a wordbreaker. 
                If we ran the contains() query on the string "qu", however, we would get a match, since the substring "qu" appears immediately 
                after a wordbreaking character.
            */

            string queryValue = "abc";
            string nonMatchValue = "user def";
            string matchValue = "user abc";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(contains({1}, '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringSV, XPathOperator.Contains, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVStringEndsWith()
        {
            string queryValue = "0001";
            string nonMatchValue = "user0002";
            string matchValue = "user0001";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(ends-with({1}, '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringSV, XPathOperator.EndsWith, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVStringStartsWith()
        {
            string queryValue = "y";
            string nonMatchValue = "xuser0002";
            string matchValue = "yuser0001";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(starts-with({1}, '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringSV, XPathOperator.StartsWith, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVStringIsPresent()
        {
            string queryValue = null;
            string nonMatchValue = null;
            string matchValue = "user0001";

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(starts-with({1}, '%'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringSV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVStringIsNotPresent()
        {
            string queryValue = null;
            string nonMatchValue = "user0001";
            string matchValue = null;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not(starts-with({1}, '%')))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringSV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Multivalued tests

        [TestMethod]
        public void TestMVStringEquals()
        {
            string queryValue = "user0001";
            List<string> nonMatchValue = new List<string>() { "user0003", "user0004" };
            List<string> matchValue = new List<string>() { "user0001", "user0002" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringMV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVStringNotEquals()
        {
            string queryValue = "user0001";
            List<string> nonMatchValue = new List<string>() { "user0001", "user0002" };
            List<string> matchValue = new List<string>() { "user0003", "user0004" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringMV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVStringContains()
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
            
            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(contains({1}, '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringMV, XPathOperator.Contains, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVStringEndsWith()
        {
            string queryValue = "0001";
            List<string> nonMatchValue = new List<string>() { "user0004", "user0003" };
            List<string> matchValue = new List<string>() { "user0002", "user0001" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(ends-with({1}, '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringMV, XPathOperator.EndsWith, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVStringStartsWith()
        {
            string queryValue = "y";
            List<string> nonMatchValue = new List<string>() { "xuser0004", "xuser0003" };
            List<string> matchValue = new List<string>() { "yuser0002", "yuser0001" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(starts-with({1}, '{2}'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringMV, XPathOperator.StartsWith, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVStringIsPresent()
        {
            string queryValue = null;
            string nonMatchValue = null;
            List<string> matchValue = new List<string>() { "user0001", "user0002" };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(starts-with({1}, '%'))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringMV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVStringIsNotPresent()
        {
            string queryValue = null;
            List<string> nonMatchValue = new List<string>() { "user0001", "user0002" };
            string matchValue = null;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeStringMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not(starts-with({1}, '%')))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeStringMV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }


        // Exception tests

        [TestMethod]
        public void TestStringGreaterThan()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeStringSV, XPathOperator.GreaterThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestStringGreaterThanOrEquals()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeStringSV, XPathOperator.GreaterThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestStringLessThan()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeStringSV, XPathOperator.LessThan);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestStringLessThanOrEquals()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeStringSV, XPathOperator.LessThanOrEquals);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        private void SubmitXpath(string value, string expected, string attributeName, XPathOperator xpathOp, QueryOperator queryOp, params ResourceObject[] matchResources)
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
