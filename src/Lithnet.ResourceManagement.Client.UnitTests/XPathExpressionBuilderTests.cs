using System;
using Lithnet.ResourceManagement.Client.XPath;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XPathExpressionBuilderTests
    {
        private readonly ResourceManagementClient client = UnitTestHelper.ServiceProvider.GetRequiredService<ResourceManagementClient>();

        public XPathExpressionBuilderTests()
        {
            this.client.DeleteResources(this.client.GetResources("/" + Constants.UnitTestObjectTypeName));
        }

        [TestMethod]
        public void TestSimpleDereference()
        {
            var v = this.client.CreateXPathBuilder()
            .FindObjectsOfType("person")
                .WhereAttribute("accountname").ValueEquals("ryan")
            .Dereference("manager")
            .BuildQuery();

            Assert.AreEqual("/Person[(AccountName = 'ryan')]/Manager", v);
        }

        [TestMethod]
        public void TestSimpleExpression()
        {
            var x = this.client.CreateXPathBuilder()
                          .FindObjectsOfType("person")
                              .WhereAttribute("accountname").ValueEquals("ryan")
                          .BuildQuery();

            Assert.AreEqual("/Person[(AccountName = 'ryan')]", x);
        }

        [TestMethod]
        public void TestSimpleExpressionAsFilter()
        {
            var z = this.client.CreateXPathBuilder()
                .FindObjectsOfType("person")
                    .WhereAttribute("accountname").ValueEquals("ryan")
              .BuildFilter();

            Assert.AreEqual("<Filter xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" Dialect=\"http://schemas.microsoft.com/2006/11/XPathFilterDialect\" xmlns=\"http://schemas.xmlsoap.org/ws/2004/09/enumeration\">/Person[(AccountName = &apos;ryan&apos;)]</Filter>", z);
        }

        [TestMethod]
        public void TestComplexExpression()
        {
            var y = this.client.CreateXPathBuilder()
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

        [TestMethod]
        public void TestComplexExpressionWithDereferencing()
        {
            var y2 = this.client.CreateXPathBuilder()
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

        [TestMethod]
        public void TestUnionQuery()
        {
            var y2 = this.client.CreateXPathBuilder()
             .FindObjectsOfAnyType()
                    .WhereAttribute("accountName").IsPresent()
             .AlsoFindObjectsOfType("group")
                   .WhereAttribute("displayedowner").IsPresent()
             .BuildQuery();

            Assert.AreEqual("/*[(starts-with(AccountName, '%'))] | /Group[(DisplayedOwner = /*)]", y2);
        }

        [TestMethod]
        public void TestUnionQuery3()
        {
            var y2 = this.client.CreateXPathBuilder()
             .FindObjectsOfAnyType()
                    .WhereAttribute("accountName").IsPresent()
             .AlsoFindObjectsOfType("group")
                   .WhereAttribute("displayedowner").IsPresent()
             .AlsoFindObjectsOfType("person")
                   .WhereAttribute("Manager").IsPresent()
             .BuildQuery();

            Assert.AreEqual("/*[(starts-with(AccountName, '%'))] | /Group[(DisplayedOwner = /*)] | /Person[(Manager = /*)]", y2);
        }

        [TestMethod]
        public void TestNewQuery()
        {
            var x = this.client.CreateXPathBuilder()
                          .FindObjectsOfType("person")
                              .WhereAttribute("accountname")
                                .ValueEquals("ryan")
                          .BuildQuery();

            Assert.AreEqual("/Person[(AccountName = 'ryan')]", x);
        }

        [TestMethod]
        public void TestReferenceEquals()
        {
            var y2 = this.client.CreateXPathBuilder()
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