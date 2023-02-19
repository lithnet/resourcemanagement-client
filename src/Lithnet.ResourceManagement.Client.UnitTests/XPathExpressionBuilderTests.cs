using System;
using Lithnet.ResourceManagement.Client.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XPathExpressionBuilderTests
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
        public void TestSimpleDereference(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var v = client.CreateXPathBuilder()
            .FindObjectsOfType("person")
                .WhereAttribute("accountname").ValueEquals("ryan")
            .Dereference("manager")
            .BuildQuery();

            Assert.AreEqual("/Person[(AccountName = 'ryan')]/Manager", v);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestSimpleExpression(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var x = client.CreateXPathBuilder()
                          .FindObjectsOfType("person")
                              .WhereAttribute("accountname").ValueEquals("ryan")
                          .BuildQuery();

            Assert.AreEqual("/Person[(AccountName = 'ryan')]", x);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestSimpleExpressionAsFilter(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var z = client.CreateXPathBuilder()
                .FindObjectsOfType("person")
                    .WhereAttribute("accountname").ValueEquals("ryan")
              .BuildFilter();

            Assert.AreEqual("<Filter xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" Dialect=\"http://schemas.microsoft.com/2006/11/XPathFilterDialect\" xmlns=\"http://schemas.xmlsoap.org/ws/2004/09/enumeration\">/Person[(AccountName = &apos;ryan&apos;)]</Filter>", z);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestComplexExpression(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var y = client.CreateXPathBuilder()
                .FindObjectsOfAnyType()
                    .StartAndGroup()
                        .WhereAttribute("accountName").IsPresent()
                        .WhereAttribute("EmployeeEndDate").ValueIsGreaterThan(DateTime.Parse("2023-01-01T00:00:00.000Z"))
                        .StartOrGroup()
                            .WhereAttribute("displayname").IsPresent()
                            .WhereAttribute("Lastname").IsNotPresent()
                            .WhereAttribute("accountname").ValueEquals("ryan")
                        .EndGroup()
                    .EndGroup()
                 .BuildQuery();

            Assert.AreEqual("/*[((starts-with(AccountName, '%')) and (EmployeeEndDate > '2023-01-01T00:00:00.000') and ((starts-with(DisplayName, '%')) or (not(starts-with(LastName, '%'))) or (AccountName = 'ryan')))]", y);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestComplexExpressionWithDereferencing(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var y2 = client.CreateXPathBuilder()
             .FindObjectsOfAnyType()
                 .StartAndGroup()
                     .WhereAttribute("accountName").IsPresent()
                     .StartOrGroup()
                         .WhereAttribute("displayname").IsPresent()
                         .WhereAttribute("Lastname").IsNotPresent()
                         .WhereAttribute("accountname").ValueEquals("ryan")
                     .EndGroup()
                 .EndGroup()
              .Dereference("Manager")
              .BuildQuery();

            Assert.AreEqual("/*[((starts-with(AccountName, '%')) and ((starts-with(DisplayName, '%')) or (not(starts-with(LastName, '%'))) or (AccountName = 'ryan')))]/Manager", y2);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestUnionQuery(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var y2 = client.CreateXPathBuilder()
             .FindObjectsOfAnyType()
                    .WhereAttribute("accountName").IsPresent()
             .AlsoFindObjectsOfType("group")
                   .WhereAttribute("displayedowner").IsPresent()
             .BuildQuery();

            Assert.AreEqual("/*[(starts-with(AccountName, '%'))] | /Group[(DisplayedOwner = /*)]", y2);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestUnionQuery3(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var y2 = client.CreateXPathBuilder()
             .FindObjectsOfAnyType()
                    .WhereAttribute("accountName").IsPresent()
             .AlsoFindObjectsOfType("group")
                   .WhereAttribute("displayedowner").IsPresent()
             .AlsoFindObjectsOfType("person")
                   .WhereAttribute("Manager").IsPresent()
             .BuildQuery();

            Assert.AreEqual("/*[(starts-with(AccountName, '%'))] | /Group[(DisplayedOwner = /*)] | /Person[(Manager = /*)]", y2);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestNewQuery(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var x = client.CreateXPathBuilder()
                          .FindObjectsOfType("person")
                              .WhereAttribute("accountname")
                                .ValueEquals("ryan")
                          .BuildQuery();

            Assert.AreEqual("/Person[(AccountName = 'ryan')]", x);
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void TestReferenceEquals(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            var y2 = client.CreateXPathBuilder()
             .FindObjectsOfAnyType()
                 .StartAndGroup()
                     .WhereAttribute("Manager")
                     .ReferenceValueMatchesSubExpression()
                        .FindObjectsOfType("Person")
                            .WhereAttribute("accountname").ValueEquals("ryan")
                    .EndSubExpression()
                 .EndGroup()
              .BuildQuery();

            Assert.AreEqual("/*[(Manager = /Person[(AccountName = 'ryan')])]", y2);
        }
    }
}