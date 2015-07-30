using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XPathAscendantExpressionTests
    {
        private ResourceManagementClient client = new ResourceManagementClient();

        public XPathAscendantExpressionTests()
        {
            client.DeleteResources(client.GetResources("/" + UnitTestHelper.ObjectTypeUnitTestObjectName));
        }

        [TestMethod]
        public void XPathAscendantExpressionTest()
        {
            string testValue1 = "test1";
            XPathPredicate predicate1 = new XPathPredicate(UnitTestHelper.AttributeStringSV, XPathOperator.Equals, testValue1);
            XPathAscendantExpression expression = new XPathAscendantExpression(UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, predicate1);
            string expected = string.Format("descendants(/{0}[({1} = '{2}')], '{3}')", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, testValue1, UnitTestHelper.AttributeReferenceSV);

            ResourceObject parentObject1 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject parentObject2 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, parentObject1.ObjectID);
            ResourceObject parentObject3 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, parentObject2.ObjectID);
            ResourceObject filterTargetObject = this.CreateTestResource(UnitTestHelper.AttributeStringSV, testValue1, UnitTestHelper.AttributeReferenceSV, parentObject3.ObjectID);
            ResourceObject childObject1 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, filterTargetObject.ObjectID);
            ResourceObject childObject2 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, childObject1.ObjectID);
            ResourceObject childObject3 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, childObject2.ObjectID);

            try
            {
                this.SubmitXpath(expression, expected, parentObject3);
            }
            finally
            {
                this.CleanupTestResources(parentObject1, parentObject2, parentObject3, filterTargetObject, childObject1, childObject2, childObject3);
            }
        }

        [TestMethod]
        public void XPathDecendantExpressionTest()
        {
            string testValue1 = "test1";
            XPathPredicate predicate1 = new XPathPredicate(UnitTestHelper.AttributeStringSV, XPathOperator.Equals, testValue1);
            XPathDescendantExpression expression = new XPathDescendantExpression(UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeReferenceSV, predicate1);
            string expected = string.Format("/{0}[descendant-in('{3}', /{0}[({1} = '{2}')])]", UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, testValue1, UnitTestHelper.AttributeReferenceSV);

            ResourceObject parentObject1 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, null);
            ResourceObject parentObject2 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, parentObject1.ObjectID);
            ResourceObject parentObject3 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, parentObject2.ObjectID);
            ResourceObject filterTargetObject = this.CreateTestResource(UnitTestHelper.AttributeStringSV, testValue1, UnitTestHelper.AttributeReferenceSV, parentObject3.ObjectID);
            ResourceObject childObject1 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, filterTargetObject.ObjectID);
            ResourceObject childObject2 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, filterTargetObject.ObjectID);
            ResourceObject childObject3 = this.CreateTestResource(UnitTestHelper.AttributeReferenceSV, filterTargetObject.ObjectID);

            try
            {
                this.SubmitXpath(expression, expected, childObject1, childObject2, childObject3);
            }
            finally
            {
                this.CleanupTestResources(parentObject1, parentObject2, parentObject3, filterTargetObject, childObject1, childObject2, childObject3);
            }
        }

        private void SubmitXpath(XPathExpression expression, string expectedXpath, params ResourceObject[] matchResources)
        {
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


        private ResourceObject CreateTestResource(string attributeName1, object value1, string attributeName2, object value2)
        {
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Attributes[attributeName1].SetValue(value1);
            resource.Attributes[attributeName2].SetValue(value2);
            resource.Save();
            return resource;
        }

        private ResourceObject CreateTestResource(string attributeName1, object value1)
        {
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Attributes[attributeName1].SetValue(value1);
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
