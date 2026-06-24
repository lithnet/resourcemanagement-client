using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    public class CreateTests
    {
        [TestCaseSource(typeof(ConnectionModeSources))]
        public void CreateFromClientSaveWithClient(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            IResourceObject resource = null;

            try
            {
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, ((ResourceObject)resource).IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);

                Assert.AreEqual(false, ((ResourceObject)resource).IsPlaceHolder);
                Assert.AreEqual(OperationType.Update, resource.ModificationType);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.IsTrue(resource.ObjectID.IsGuid);
                Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
            }
            finally
            {
                if (resource != null && !((ResourceObject)resource).IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void CreateFromClientSaveOnObject(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            IResourceObject resource = null;

            try
            {
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, ((ResourceObject)resource).IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();
                Assert.AreEqual(false, ((ResourceObject)resource).IsPlaceHolder);
                Assert.AreEqual(OperationType.Update, resource.ModificationType);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.IsTrue(resource.ObjectID.IsGuid);
                Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
            }
            finally
            {
                if (resource != null && !((ResourceObject)resource).IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void CreateByConstructorSaveOnObject(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            IResourceObject resource = null;

            try
            {
                resource = new ResourceObject(Constants.UnitTestObjectTypeName, client.ClientFactory);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, ((ResourceObject)resource).IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();
                Assert.AreEqual(false, ((ResourceObject)resource).IsPlaceHolder);
                Assert.AreEqual(OperationType.Update, resource.ModificationType);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.IsTrue(resource.ObjectID.IsGuid);
                Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
            }
            finally
            {
                if (resource != null && !((ResourceObject)resource).IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void CreateByConstructorSaveWithClient(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            IResourceObject resource = null;

            try
            {
                resource = new ResourceObject(Constants.UnitTestObjectTypeName, client.ClientFactory);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, ((ResourceObject)resource).IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);
                Assert.AreEqual(false, ((ResourceObject)resource).IsPlaceHolder);
                Assert.AreEqual(OperationType.Update, resource.ModificationType);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.IsTrue(resource.ObjectID.IsGuid);
                Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
            }
            finally
            {
                if (resource != null && !((ResourceObject)resource).IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void CreateByConstructorSaveWithClientComposite(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            List<IResourceObject> resources = new List<IResourceObject>();

            for (int i = 0; i < 5; i++)
            {
                IResourceObject resource = new ResourceObject(Constants.UnitTestObjectTypeName, client.ClientFactory);
                Assert.AreEqual(OperationType.Create, resource.ModificationType);
                Assert.AreEqual(true, ((ResourceObject)resource).IsPlaceHolder);
                UnitTestHelper.PopulateTestUserData(resource);
                resources.Add(resource);
            }

            try
            {
                client.SaveResources(resources);
                foreach (IResourceObject resource in resources)
                {
                    Assert.AreEqual(false, ((ResourceObject)resource).IsPlaceHolder);
                    Assert.AreEqual(OperationType.Update, resource.ModificationType);
                    Assert.AreEqual(0, resource.PendingChanges.Count);
                    Assert.IsTrue(resource.ObjectID.IsGuid);
                    Assert.AreNotEqual(resource.ObjectID.GetGuid(), Guid.Empty);

                    IResourceObject resourceFetched = client.GetResource(resource.ObjectID);

                    UnitTestHelper.ValidateTestUserData(resourceFetched);
                }
            }
            finally
            {
                client.DeleteResources(resources.Where(r => r != null && !((ResourceObject)r).IsPlaceHolder));
            }
        }
    }
}
