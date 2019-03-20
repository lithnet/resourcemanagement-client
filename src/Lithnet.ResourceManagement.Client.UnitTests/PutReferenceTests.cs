using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutReferenceTests
    {
        [TestMethod]
        public void AddReferenceSV()
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
                resource.Attributes[UnitTestHelper.AttributeReferenceSV].SetValue(UnitTestHelper.TestDataReference1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataReference1, resource.Attributes[UnitTestHelper.AttributeReferenceSV].ReferenceValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataReference1, resource.Attributes[UnitTestHelper.AttributeReferenceSV].ReferenceValue);
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
        public void ModifyReferenceSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeReferenceSV].SetValue(UnitTestHelper.TestDataReference1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeReferenceSV].SetValue(UnitTestHelper.TestDataReference2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataReference2, resource.Attributes[UnitTestHelper.AttributeReferenceSV].ReferenceValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataReference2, resource.Attributes[UnitTestHelper.AttributeReferenceSV].ReferenceValue);
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
        public void DeleteReferenceSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeReferenceSV].SetValue(UnitTestHelper.TestDataReference1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeReferenceSV].RemoveValue(UnitTestHelper.TestDataReference1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeReferenceSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeReferenceSV].IsNull);
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
        public void DeleteAllValueReferenceSV()
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
                resource.Attributes[UnitTestHelper.AttributeReferenceSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeReferenceSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeReferenceSV].IsNull);
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
        public void AddFirstReferenceMV()
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
                resource.Attributes[UnitTestHelper.AttributeReferenceMV].AddValue(UnitTestHelper.TestDataReference2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new UniqueIdentifier[1] { UnitTestHelper.TestDataReference2 }, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new UniqueIdentifier[1] { UnitTestHelper.TestDataReference2 }, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);
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
        public void AddSecondReferenceMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeReferenceMV].AddValue(UnitTestHelper.TestDataReference1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeReferenceMV].AddValue(UnitTestHelper.TestDataReference2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new UniqueIdentifier[2] { UnitTestHelper.TestDataReference1, UnitTestHelper.TestDataReference2 }, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new UniqueIdentifier[2] { UnitTestHelper.TestDataReference1, UnitTestHelper.TestDataReference2 }, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);
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
        public void ReplaceReferenceMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeReferenceMV].SetValue(UnitTestHelper.TestDataReference1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeReferenceMV].SetValue(UnitTestHelper.TestDataReference2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(UnitTestHelper.TestDataReference2MV, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(UnitTestHelper.TestDataReference2MV, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);
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
        public void DeleteFirstValueReferenceMV()
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
                resource.Attributes[UnitTestHelper.AttributeReferenceMV].RemoveValue(UnitTestHelper.TestDataReference1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new UniqueIdentifier[2] { UnitTestHelper.TestDataReference1MV[1], UnitTestHelper.TestDataReference1MV[2] }, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new UniqueIdentifier[2] { UnitTestHelper.TestDataReference1MV[1], UnitTestHelper.TestDataReference1MV[2] }, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);
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
        public void DeleteAllValueReferenceMV()
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
                resource.Attributes[UnitTestHelper.AttributeReferenceMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeReferenceMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeReferenceMV].IsNull);
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
