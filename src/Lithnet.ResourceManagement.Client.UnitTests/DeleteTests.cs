using System.Collections.Generic;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class DeleteTests
    {
        [TestMethod]
        public void DeleteByID()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Save();

            client.DeleteResource(resource.ObjectID);

            try
            {
                client.GetResource(resource.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }
        }

        [TestMethod]
        public void DeleteByGuid()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Save();

            client.DeleteResource(resource.ObjectID.GetGuid());

            try
            {
                client.GetResource(resource.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }
        }

        [TestMethod]
        public void DeleteByString()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Save();

            client.DeleteResource(resource.ObjectID.Value);

            try
            {
                client.GetResource(resource.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }
        }

        [TestMethod]
        public void DeleteByObject()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Save();

            client.DeleteResource(resource);

            try
            {
                client.GetResource(resource.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }
        }

        [TestMethod]
        public void DeleteByStringNonExistant()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            try
            {
                client.DeleteResource("f970bdf5-7b41-4618-82e6-ff16d34d2e41");
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch (PermissionDeniedException)
            {
            }
        }

        [TestMethod]
        public void CompositeDeleteByObjectTest()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            ResourceObject resource1 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource1.Save();
            resource1 = client.GetResource(resource1.ObjectID);

            ResourceObject resource2 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource2.Save();
            resource2 = client.GetResource(resource2.ObjectID);

            ResourceObject resource3 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource3.Save();
            resource3 = client.GetResource(resource3.ObjectID);

            client.DeleteResources(new List<ResourceObject>() { resource1, resource2, resource3 });

            try
            {
                client.GetResource(resource1.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }

            try
            {
                client.GetResource(resource2.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }

            try
            {
                client.GetResource(resource3.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }

        }

        [TestMethod]
        public void CompositeDeleteByIDTest()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            ResourceObject resource1 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource1.Save();
            resource1 = client.GetResource(resource1.ObjectID);

            ResourceObject resource2 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource2.Save();
            resource2 = client.GetResource(resource2.ObjectID);

            ResourceObject resource3 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource3.Save();
            resource3 = client.GetResource(resource3.ObjectID);

            client.DeleteResources(new List<UniqueIdentifier>() { resource1.ObjectID, resource2.ObjectID, resource3.ObjectID });

            try
            {
                client.GetResource(resource1.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }

            try
            {
                client.GetResource(resource2.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }

            try
            {
                client.GetResource(resource3.ObjectID);
                Assert.Fail("The object was still present after the delete operation");
            }
            catch (ResourceNotFoundException)
            {
            }

        }
    }
}
