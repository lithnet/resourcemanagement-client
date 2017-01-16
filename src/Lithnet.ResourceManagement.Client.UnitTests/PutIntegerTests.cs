using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutIntegerTests
    {
        [TestMethod]
        public void AddIntegerSV()
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
                resource.Attributes[UnitTestHelper.AttributeIntegerSV].SetValue(UnitTestHelper.TestDataInteger1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataInteger1, resource.Attributes[UnitTestHelper.AttributeIntegerSV].IntegerValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataInteger1, resource.Attributes[UnitTestHelper.AttributeIntegerSV].IntegerValue);
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
        public void ModifyIntegerSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeIntegerSV].SetValue(UnitTestHelper.TestDataInteger1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeIntegerSV].SetValue(UnitTestHelper.TestDataInteger2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataInteger2, resource.Attributes[UnitTestHelper.AttributeIntegerSV].IntegerValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataInteger2, resource.Attributes[UnitTestHelper.AttributeIntegerSV].IntegerValue);
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
        public void DeleteIntegerSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeIntegerSV].SetValue(UnitTestHelper.TestDataInteger1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeIntegerSV].RemoveValue(UnitTestHelper.TestDataInteger1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeIntegerSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeIntegerSV].IsNull);
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
        public void DeleteAllValueIntegerSV()
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
                resource.Attributes[UnitTestHelper.AttributeIntegerSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeIntegerSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeIntegerSV].IsNull);
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
        public void AddFirstIntegerMV()
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
                resource.Attributes[UnitTestHelper.AttributeIntegerMV].AddValue(UnitTestHelper.TestDataInteger2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new long[1] { UnitTestHelper.TestDataInteger2 }, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new long[1] { UnitTestHelper.TestDataInteger2 }, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);
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
        public void AddSecondIntegerMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeIntegerMV].AddValue(UnitTestHelper.TestDataInteger1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeIntegerMV].AddValue(UnitTestHelper.TestDataInteger2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new long[2] { UnitTestHelper.TestDataInteger1, UnitTestHelper.TestDataInteger2 }, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new long[2] { UnitTestHelper.TestDataInteger1, UnitTestHelper.TestDataInteger2 }, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);
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
        public void ReplaceIntegerMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeIntegerMV].SetValue(UnitTestHelper.TestDataInteger1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeIntegerMV].SetValue(UnitTestHelper.TestDataInteger2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(UnitTestHelper.TestDataInteger2MV, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(UnitTestHelper.TestDataInteger2MV, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);
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
        public void DeleteFirstValueIntegerMV()
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
                resource.Attributes[UnitTestHelper.AttributeIntegerMV].RemoveValue(UnitTestHelper.TestDataInteger1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new long[2] { UnitTestHelper.TestDataInteger1MV[1], UnitTestHelper.TestDataInteger1MV[2] }, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new long[2] { UnitTestHelper.TestDataInteger1MV[1], UnitTestHelper.TestDataInteger1MV[2] }, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);
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
        public void DeleteAllValueIntegerMV()
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
                resource.Attributes[UnitTestHelper.AttributeIntegerMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeIntegerMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeIntegerMV].IsNull);
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
