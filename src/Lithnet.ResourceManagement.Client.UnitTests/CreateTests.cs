﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class CreateTests
    {
        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void CreateFromClientSaveWithClient(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void CreateFromClientSaveOnObject(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void CreateByConstructorSaveOnObject(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                resource = new ResourceObject(Constants.UnitTestObjectTypeName, client.ClientFactory);
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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void CreateByConstructorSaveWithClient(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                resource = new ResourceObject(Constants.UnitTestObjectTypeName, client.ClientFactory);
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

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void CreateByConstructorSaveWithClientComposite(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            List<ResourceObject> resources = new List<ResourceObject>();

            for (int i = 0; i < 5; i++)
            {
                ResourceObject resource = new ResourceObject(Constants.UnitTestObjectTypeName, client.ClientFactory);
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
