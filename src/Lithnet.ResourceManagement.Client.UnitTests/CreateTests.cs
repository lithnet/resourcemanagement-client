using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class CreateTests
    {
        [TestMethod]
        public void CreateFromClientSaveWithClient()
        {
            UnitTestHelper.PrepareRMSForUnitTests();
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, resource.IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);

                Assert.AreEqual(false, resource.IsPlaceHolder);
                Assert.AreEqual(OperationType.Update, resource.ModificationType);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.IsTrue(resource.ObjectID.IsGuid);
                Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestMethod]
        public void CreateFromClientSaveOnObject()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, resource.IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();
                Assert.AreEqual(false, resource.IsPlaceHolder);
                Assert.AreEqual(OperationType.Update, resource.ModificationType);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.IsTrue(resource.ObjectID.IsGuid);
                Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestMethod]
        public void CreateByConstructorSaveOnObject()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                resource = new ResourceObject(UnitTestHelper.ObjectTypeUnitTestObjectName, client);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, resource.IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();
                Assert.AreEqual(false, resource.IsPlaceHolder);
                Assert.AreEqual(OperationType.Update, resource.ModificationType);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.IsTrue(resource.ObjectID.IsGuid);
                Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestMethod]
        public void CreateByConstructorSaveWithClient()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                resource = new ResourceObject(UnitTestHelper.ObjectTypeUnitTestObjectName, client);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, resource.IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);
                Assert.AreEqual(false, resource.IsPlaceHolder);
                Assert.AreEqual(OperationType.Update, resource.ModificationType);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.IsTrue(resource.ObjectID.IsGuid);
                Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestMethod]
        public void CreateByConstructorSaveWithClientComposite()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            List<ResourceObject> resources = new List<ResourceObject>();

            for (int i = 0; i < 5; i++)
            {
                ResourceObject resource = new ResourceObject(UnitTestHelper.ObjectTypeUnitTestObjectName, client);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, resource.IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                resources.Add(resource);
            }

            try
            {
                client.SaveResources(resources);
                foreach (ResourceObject resource in resources)
                {
                    Assert.AreEqual(false, resource.IsPlaceHolder);
                    Assert.AreEqual(OperationType.Update, resource.ModificationType);
                    Assert.AreEqual(0, resource.PendingChanges.Count);
                    Assert.IsTrue(resource.ObjectID.IsGuid);
                    Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                    ResourceObject resourceFetched = client.GetResource(resource.ObjectID);

                    UnitTestHelper.ValidateTestUserData(resourceFetched);
                }
            }
            finally
            {
                client.DeleteResources(resources.Where(r => r != null && !r.IsPlaceHolder));
            }
        }
    }
}
