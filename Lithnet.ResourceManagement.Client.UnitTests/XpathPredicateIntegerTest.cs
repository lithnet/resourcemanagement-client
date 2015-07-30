using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathQueryBuilderIntegerTests
    {
        private ResourceManagementClient client = new ResourceManagementClient();

        public XpathQueryBuilderIntegerTests()
        {
            client.DeleteResources(client.GetResources("/" + UnitTestHelper.ObjectTypeUnitTestObjectName));
        }

        // Single-value tests

        [TestMethod]
        public void TestSVIntegerEquals()
        {
            object queryValue = 1;
            object nonMatchValue = 2;
            object matchValue = 1;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerSV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerNotEquals()
        {
            object queryValue = 1;
            object nonMatchValue = 1;
            object matchValue = 2;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} != {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerSV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        //[TestMethod]
        //public void FindLowestSupportedLong()
        //{
        //    ResourceManagementClient client = new ResourceManagementClient();

        //    System.IO.StreamWriter q = new System.IO.StreamWriter(@"D:\temp\test.log", false);
        //    q.AutoFlush = true;
        //    //9203939036854775806
        //    //1939036854775806 last rejected
        //    // 939036854775806 last accepted
        //    for (long i = 1000000000000001; i > 0; i = i - 1)
        //    {
        //        string expectedXpath = string.Format("/{0}[({1} <= {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, i);

        //        try
        //        {
        //            client.GetResources(expectedXpath);
        //            q.WriteLine("Accepted {0}", i);
        //            break;
        //        }
        //        catch
        //        {
        //        }

        //        q.WriteLine("Rejected {0}", i);
        //    }
        //}

        [TestMethod]
        public void TestSVIntegerIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            object matchValue = 1;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, XPathPredicate.MaxLong);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerSV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerIsNotPresent()
        {
            object queryValue = null;
            object nonMatchValue = 1;
            object matchValue = null;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= {2}))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, XPathPredicate.MaxLong);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerSV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerGreaterThan()
        {
            object queryValue = 10;
            object nonMatchValue = 5;
            object matchValue = 11;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerSV, XPathOperator.GreaterThan, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerGreaterThanOrEquals()
        {
            object queryValue = 10;
            object nonMatchValue = 5;
            object matchValue = 10;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerSV, XPathOperator.GreaterThanOrEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerLessThan()
        {
            object queryValue = 10;
            object nonMatchValue = 15;
            object matchValue = 9;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerSV, XPathOperator.LessThan, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestSVIntegerLessThanOrEquals()
        {
            object queryValue = 10;
            object nonMatchValue = 15;
            object matchValue = 10;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerSV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerSV, XPathOperator.LessThanOrEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Multivalued tests

        [TestMethod]
        public void TestMVIntegerEquals()
        {
            object queryValue = 1;
            List<long> nonMatchValue = new List<long>() { 2, 3 };
            List<long> matchValue = new List<long>() { 1, 4 };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerMV, XPathOperator.Equals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerNotEquals()
        {
            object queryValue = 1;
            List<long> nonMatchValue = new List<long>() { 1, 3 };
            List<long> matchValue = new List<long>() { 3, 4 };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = {2}))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerMV, XPathOperator.NotEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerIsPresent()
        {
            object queryValue = null;
            object nonMatchValue = null;
            List<long> matchValue = new List<long>() { 1, 2 };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerMV, XPathPredicate.MaxLong);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerMV, XPathOperator.IsPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerIsNotPresent()
        {
            object queryValue = null;
            List<long> nonMatchValue = new List<long>() { 1, 2 };
            object matchValue = null;

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= {2}))]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerMV, XPathPredicate.MaxLong);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerMV, XPathOperator.IsNotPresent, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerGreaterThan()
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 9, 8 };
            List<object> matchValue = new List<object>() { 9, 11 };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerMV, XPathOperator.GreaterThan, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerGreaterThanOrEquals()
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 9, 8 };
            List<object> matchValue = new List<object>() { 9, 10 };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerMV, XPathOperator.GreaterThanOrEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerLessThan()
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 15, 20 };
            List<object> matchValue = new List<object>() { 9, 20 };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerMV, XPathOperator.LessThan, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [TestMethod]
        public void TestMVIntegerLessThanOrEquals()
        {
            object queryValue = 10;
            List<object> nonMatchValue = new List<object>() { 15, 20 };
            List<object> matchValue = new List<object>() { 10, 20 };

            ResourceObject matchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, matchValue);
            ResourceObject nonMatchResource = this.CreateTestResource(UnitTestHelper.AttributeIntegerMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeIntegerMV, queryValue);
                this.SubmitXpath(queryValue, expected, UnitTestHelper.AttributeIntegerMV, XPathOperator.LessThanOrEquals, QueryOperator.And, matchResource);
            }
            finally
            {
                this.CleanupTestResources(matchResource, nonMatchResource);
            }
        }



        // Exception tests

        [TestMethod]
        public void TestMVIntegerContains()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeIntegerMV, XPathOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVIntegerEndsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeIntegerMV, XPathOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVIntegerStartsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeIntegerMV, XPathOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerContains()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeIntegerSV, XPathOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerEndsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeIntegerSV, XPathOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVIntegerStartsWith()
        {
            try
            {
                XPathPredicate predicate = new XPathPredicate(UnitTestHelper.AttributeIntegerSV, XPathOperator.StartsWith);
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
