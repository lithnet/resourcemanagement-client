using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathPredicateDateTimeTests
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeEquals(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "3000-01-01T00:00:00.000";
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeNotEquals(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            object nonMatchValue = null;
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeGreaterThan(ConnectionMode connectionMode)
        {
            object queryValue = "3000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3100-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.GreaterThan, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeGreaterThanFunction(ConnectionMode connectionMode)
        {
            object queryValue = "current-dateTime()";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.GreaterThan, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeGreaterThanOrEquals(ConnectionMode connectionMode)
        {
            object queryValue = "3000-01-01T00:00:00.000";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeGreaterThanOrEqualsFunction(ConnectionMode connectionMode)
        {
            object queryValue = "current-dateTime()";
            object nonMatchValue = "2000-01-01T00:00:00.000";
            object matchValue = "3000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeLessThan(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2100-01-01T00:00:00.000";
            object matchValue = "1900-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.LessThan, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeLessThanFunction(ConnectionMode connectionMode)
        {
            object queryValue = "current-dateTime()";
            object nonMatchValue = "3000-01-01T00:00:00.000";
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.LessThan, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeLessThanOrEquals(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            object nonMatchValue = "2100-01-01T00:00:00.000";
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.LessThanOrEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestSVDateTimeLessThanOrEqualsFunction(ConnectionMode connectionMode)
        {
            object queryValue = "current-dateTime()";
            object nonMatchValue = "3000-01-01T00:00:00.000";
            object matchValue = "2000-01-01T00:00:00.000";

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeSV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeSV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeSVDef, ComparisonOperator.LessThanOrEquals, GroupOperator.And, connectionMode, matchResource);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeEquals(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            ;
            List<string> nonMatchValue = new List<string>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<string> matchValue = new List<string>() { "2300-01-01T00:00:00.000", "2000-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} = '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.Equals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeNotEquals(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<string> nonMatchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };
            List<string> matchValue = new List<string>() { "2200-01-01T00:00:00.000", "2300-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} = '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue, matchResource);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.NotEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeIsPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            object nonMatchValue = null;
            List<string> matchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.IsPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeIsNotPresent(ConnectionMode connectionMode)
        {
            object queryValue = null;
            List<string> nonMatchValue = new List<string>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };
            object matchValue = null;

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[(not({1} <= '{2}'))]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, XPathQuery.MaxDate);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.IsNotPresent, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeGreaterThan(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.GreaterThan, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeGreaterThanFunction(ConnectionMode connectionMode)
        {
            object queryValue = "current-dateTime()";
            List<object> nonMatchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} > {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.GreaterThan, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeGreaterThanOrEquals(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeGreaterThanOrEqualsFunction(ConnectionMode connectionMode)
        {
            object queryValue = "current-dateTime()";
            List<object> nonMatchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2000-01-01T00:00:00.000", "2100-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} >= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.GreaterThanOrEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeLessThan(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.LessThan, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeLessThanFunction(ConnectionMode connectionMode)
        {
            object queryValue = "current-dateTime()";
            List<object> nonMatchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "1900-01-01T00:00:00.000", "1800-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} < {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.LessThan, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeLessThanOrEquals(ConnectionMode connectionMode)
        {
            object queryValue = "2000-01-01T00:00:00.000";
            List<object> nonMatchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2000-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= '{2}')]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.LessThanOrEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void TestMVDateTimeLessThanOrEqualsFunction(ConnectionMode connectionMode)
        {
            object queryValue = "current-dateTime()";
            List<object> nonMatchValue = new List<object>() { "2100-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };
            List<object> matchValue = new List<object>() { "2000-01-01T00:00:00.000", "2200-01-01T00:00:00.000" };

            ResourceObject matchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, matchValue);
            ResourceObject nonMatchResource = UnitTestHelper.CreateTestResource(Constants.AttributeDateTimeMV, nonMatchValue);

            try
            {
                string expected = string.Format("/{0}[({1} <= {2})]", Constants.UnitTestObjectTypeName, Constants.AttributeDateTimeMV, queryValue);
                this.SubmitXpath(queryValue, expected, Constants.AttributeDateTimeMVDef, ComparisonOperator.LessThanOrEquals, GroupOperator.And, connectionMode, matchResource);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchResource, nonMatchResource);
            }
        }

        // Exception tests

        [TestMethod]
        public void TestMVDateTimeContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeDateTimeMVDef, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVDateTimeEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeDateTimeMVDef, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestMVDateTimeStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeDateTimeMVDef, ComparisonOperator.StartsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeContains()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeDateTimeSVDef, ComparisonOperator.Contains);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeEndsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeDateTimeSVDef, ComparisonOperator.EndsWith);
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch { }
        }

        [TestMethod]
        public void TestSVDateTimeStartsWith()
        {
            try
            {
                XPathQuery predicate = new XPathQuery(Constants.AttributeDateTimeSVDef, ComparisonOperator.StartsWith);
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