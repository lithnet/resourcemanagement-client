using System.Collections.Generic;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{

    public class DeleteTests
    {
        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteEmptyListResource(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            client.DeleteResources(new List<ResourceObject>());
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteEmptyListUniqueIdentifier(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            client.DeleteResources(new List<UniqueIdentifier>());
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteByID(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
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

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteByGuid(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
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

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteByString(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
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

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteByObject(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
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

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteByStringNonExistant(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            try
            {
                client.DeleteResource("f970bdf5-7b41-4618-82e6-ff16d34d2e41");
                Assert.Fail("The expectedXpath exception was not thrown");
            }
            catch (PermissionDeniedException)
            {
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void CompositeDeleteByObjectTest(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            ResourceObject resource1 = client.CreateResource(Constants.UnitTestObjectTypeName);
            resource1.Save();
            resource1 = client.GetResource(resource1.ObjectID);

            ResourceObject resource2 = client.CreateResource(Constants.UnitTestObjectTypeName);
            resource2.Save();
            resource2 = client.GetResource(resource2.ObjectID);

            ResourceObject resource3 = client.CreateResource(Constants.UnitTestObjectTypeName);
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

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void CompositeDeleteByIDTest(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            ResourceObject resource1 = client.CreateResource(Constants.UnitTestObjectTypeName);
            resource1.Save();
            resource1 = client.GetResource(resource1.ObjectID);

            ResourceObject resource2 = client.CreateResource(Constants.UnitTestObjectTypeName);
            resource2.Save();
            resource2 = client.GetResource(resource2.ObjectID);

            ResourceObject resource3 = client.CreateResource(Constants.UnitTestObjectTypeName);
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
