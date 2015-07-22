using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Lithnet.ResourceManagement.Client;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using Microsoft.ResourceManagement.WebServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutStringTests
    {
        [TestMethod]
        public void AddStringSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringSV].SetValue(UnitTestHelper.TestDataString1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataString1, resource.Attributes[UnitTestHelper.AttributeStringSV].StringValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataString1, resource.Attributes[UnitTestHelper.AttributeStringSV].StringValue);
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
        public void ModifyStringSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeStringSV].SetValue(UnitTestHelper.TestDataString1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringSV].SetValue(UnitTestHelper.TestDataString2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataString2, resource.Attributes[UnitTestHelper.AttributeStringSV].StringValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataString2, resource.Attributes[UnitTestHelper.AttributeStringSV].StringValue);
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
        public void DeleteStringSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeStringSV].SetValue(UnitTestHelper.TestDataString1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringSV].RemoveValue(UnitTestHelper.TestDataString1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeStringSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeStringSV].IsNull);
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
        public void DeleteAllValueStringSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeStringSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeStringSV].IsNull);
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
        public void AddFirstStringMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringMV].AddValue(UnitTestHelper.TestDataString2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[1] { UnitTestHelper.TestDataString2 }, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[1] { UnitTestHelper.TestDataString2 }, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);
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
        public void AddSecondStringMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeStringMV].AddValue(UnitTestHelper.TestDataString1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringMV].AddValue(UnitTestHelper.TestDataString2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[2] { UnitTestHelper.TestDataString1, UnitTestHelper.TestDataString2 }, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[2] { UnitTestHelper.TestDataString1, UnitTestHelper.TestDataString2 }, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);
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
        public void ReplaceStringMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeStringMV].SetValue(UnitTestHelper.TestDataString1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringMV].SetValue(UnitTestHelper.TestDataString2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(UnitTestHelper.TestDataString2MV, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(UnitTestHelper.TestDataString2MV, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);
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
        public void DeleteFirstValueStringMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringMV].RemoveValue(UnitTestHelper.TestDataString1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[2] { UnitTestHelper.TestDataString1MV[1], UnitTestHelper.TestDataString1MV[2] }, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[2] { UnitTestHelper.TestDataString1MV[1], UnitTestHelper.TestDataString1MV[2] }, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);
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
        public void DeleteAllValueStringMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeStringMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeStringMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeStringMV].IsNull);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }
    }
}
