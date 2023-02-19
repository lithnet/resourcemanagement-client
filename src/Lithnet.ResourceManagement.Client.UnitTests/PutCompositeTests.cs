using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutCompositeTests
    {
        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void PutCompositeTestMultipleUpdates(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource1 = null;
            ResourceObject resource2 = null;

            try
            {
                // Create the empty object
                resource1 = client.CreateResource(Constants.UnitTestObjectTypeName);
                UnitTestHelper.PopulateTestUserData(resource1);
                client.SaveResource(resource1);

                resource2 = client.CreateResource(Constants.UnitTestObjectTypeName);
                UnitTestHelper.PopulateTestUserData(resource2);
                client.SaveResource(resource2);

                resource1.Refresh();
                resource2.Refresh();

                // Make the changes
                resource1.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString2);
                resource2.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString3);

                client.SaveResources(new List<ResourceObject>() { resource1, resource2 });

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource1.PendingChanges.Count);
                Assert.AreEqual(0, resource2.PendingChanges.Count);

                resource1 = client.GetResource(resource1.ObjectID);
                resource2 = client.GetResource(resource2.ObjectID);

                Assert.AreEqual(Constants.TestDataString2, resource1.Attributes[Constants.AttributeStringSV].StringValue);
                Assert.AreEqual(Constants.TestDataString3, resource2.Attributes[Constants.AttributeStringSV].StringValue);
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
