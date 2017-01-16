using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutDateTimeTests
    {
        [TestMethod]
        public void AddDateTimeSV()
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
                resource.Attributes[UnitTestHelper.AttributeDateTimeSV].SetValue(UnitTestHelper.TestDataDateTime1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataDateTime1, resource.Attributes[UnitTestHelper.AttributeDateTimeSV].DateTimeValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataDateTime1, resource.Attributes[UnitTestHelper.AttributeDateTimeSV].DateTimeValue);
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
        public void ModifyDateTimeSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeDateTimeSV].SetValue(UnitTestHelper.TestDataDateTime1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeDateTimeSV].SetValue(UnitTestHelper.TestDataDateTime2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataDateTime2, resource.Attributes[UnitTestHelper.AttributeDateTimeSV].DateTimeValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataDateTime2, resource.Attributes[UnitTestHelper.AttributeDateTimeSV].DateTimeValue);
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
        public void DeleteDateTimeSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeDateTimeSV].SetValue(UnitTestHelper.TestDataDateTime1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeDateTimeSV].RemoveValue(UnitTestHelper.TestDataDateTime1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeDateTimeSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeDateTimeSV].IsNull);
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
        public void DeleteAllValueDateTimeSV()
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
                resource.Attributes[UnitTestHelper.AttributeDateTimeSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeDateTimeSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeDateTimeSV].IsNull);
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
        public void AddFirstDateTimeMV()
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
                resource.Attributes[UnitTestHelper.AttributeDateTimeMV].AddValue(UnitTestHelper.TestDataDateTime2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new DateTime[1] { UnitTestHelper.TestDataDateTime2 }, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new DateTime[1] { UnitTestHelper.TestDataDateTime2 }, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);
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
        public void AddSecondDateTimeMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeDateTimeMV].AddValue(UnitTestHelper.TestDataDateTime1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeDateTimeMV].AddValue(UnitTestHelper.TestDataDateTime2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new DateTime[2] { UnitTestHelper.TestDataDateTime1, UnitTestHelper.TestDataDateTime2 }, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new DateTime[2] { UnitTestHelper.TestDataDateTime1, UnitTestHelper.TestDataDateTime2 }, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);
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
        public void ReplaceDateTimeMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeDateTimeMV].SetValue(UnitTestHelper.TestDataDateTime1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeDateTimeMV].SetValue(UnitTestHelper.TestDataDateTime2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(UnitTestHelper.TestDataDateTime2MV, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(UnitTestHelper.TestDataDateTime2MV, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);
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
        public void DeleteFirstValueDateTimeMV()
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
                resource.Attributes[UnitTestHelper.AttributeDateTimeMV].RemoveValue(UnitTestHelper.TestDataDateTime1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new DateTime[2] { UnitTestHelper.TestDataDateTime1MV[1], UnitTestHelper.TestDataDateTime1MV[2] }, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new DateTime[2] { UnitTestHelper.TestDataDateTime1MV[1], UnitTestHelper.TestDataDateTime1MV[2] }, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);
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
        public void DeleteAllValueDateTimeMV()
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
                resource.Attributes[UnitTestHelper.AttributeDateTimeMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeDateTimeMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeDateTimeMV].IsNull);
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
