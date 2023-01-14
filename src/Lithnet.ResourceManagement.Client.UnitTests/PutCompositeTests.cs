using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutCompositeTests
    {
        [TestMethod]
        public void PutCompositeTestMultipleUpdates()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource1 = null;
            ResourceObject resource2 = null;

            try
            {
                // Create the empty object
                resource1 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                UnitTestHelper.PopulateTestUserData(resource1);
                client.SaveResource(resource1);

                resource2 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                UnitTestHelper.PopulateTestUserData(resource2);
                client.SaveResource(resource2);

                resource1.Refresh();
                resource2.Refresh();

                // Make the changes
                resource1.Attributes[UnitTestHelper.AttributeStringSV].SetValue(UnitTestHelper.TestDataString2);
                resource2.Attributes[UnitTestHelper.AttributeStringSV].SetValue(UnitTestHelper.TestDataString3);

                client.SaveResources(new List<ResourceObject>() { resource1, resource2 });

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource1.PendingChanges.Count);
                Assert.AreEqual(0, resource2.PendingChanges.Count);

                resource1 = client.GetResource(resource1.ObjectID);
                resource2 = client.GetResource(resource2.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataString2, resource1.Attributes[UnitTestHelper.AttributeStringSV].StringValue);
                Assert.AreEqual(UnitTestHelper.TestDataString3, resource2.Attributes[UnitTestHelper.AttributeStringSV].StringValue);
            }
            finally
            {
                if (resource1 != null && !resource1.IsPlaceHolder)
                {
                    client.DeleteResource(resource1);
                }

                if (resource2 != null && !resource2.IsPlaceHolder)
                {
                    client.DeleteResource(resource2);
                }
            }
        }
    }
}
