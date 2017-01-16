using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutBooleanTests
    {
        [TestMethod]
        public void AddBooleanSV()
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
                resource.Attributes[UnitTestHelper.AttributeBooleanSV].SetValue(UnitTestHelper.TestDataBooleanTrue);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataBooleanTrue, resource.Attributes[UnitTestHelper.AttributeBooleanSV].BooleanValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataBooleanTrue, resource.Attributes[UnitTestHelper.AttributeBooleanSV].BooleanValue);
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
        public void ModifyBooleanSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeBooleanSV].SetValue(UnitTestHelper.TestDataBooleanTrue);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeBooleanSV].SetValue(UnitTestHelper.TestDataBooleanFalse);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(UnitTestHelper.TestDataBooleanFalse, resource.Attributes[UnitTestHelper.AttributeBooleanSV].BooleanValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(UnitTestHelper.TestDataBooleanFalse, resource.Attributes[UnitTestHelper.AttributeBooleanSV].BooleanValue);
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
        public void DeleteBooleanSV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeBooleanSV].SetValue(UnitTestHelper.TestDataBooleanTrue);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeBooleanSV].RemoveValue(UnitTestHelper.TestDataBooleanTrue);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBooleanSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBooleanSV].IsNull);
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
        public void DeleteAllValueBooleanSV()
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
                resource.Attributes[UnitTestHelper.AttributeBooleanSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBooleanSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBooleanSV].IsNull);
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
