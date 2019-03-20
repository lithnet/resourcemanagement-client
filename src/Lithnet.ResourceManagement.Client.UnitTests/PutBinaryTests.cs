using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutBinaryTests
    {
        [TestMethod]
        public void AddBinarySV()
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
                resource.Attributes[UnitTestHelper.AttributeBinarySV].SetValue(UnitTestHelper.TestDataBinary1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(UnitTestHelper.TestDataBinary1, resource.Attributes[UnitTestHelper.AttributeBinarySV].BinaryValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(UnitTestHelper.TestDataBinary1, resource.Attributes[UnitTestHelper.AttributeBinarySV].BinaryValue);
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
        public void ModifyBinarySV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeBinarySV].SetValue(UnitTestHelper.TestDataBinary1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeBinarySV].SetValue(UnitTestHelper.TestDataBinary2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(UnitTestHelper.TestDataBinary2, resource.Attributes[UnitTestHelper.AttributeBinarySV].BinaryValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(UnitTestHelper.TestDataBinary2, resource.Attributes[UnitTestHelper.AttributeBinarySV].BinaryValue);
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
        public void DeleteBinarySV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeBinarySV].SetValue(UnitTestHelper.TestDataBinary1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeBinarySV].RemoveValue(UnitTestHelper.TestDataBinary1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBinarySV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBinarySV].IsNull);
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
        public void DeleteAllValueBinarySV()
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
                resource.Attributes[UnitTestHelper.AttributeBinarySV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBinarySV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBinarySV].IsNull);
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
        public void AddFirstBinaryMV()
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
                resource.Attributes[UnitTestHelper.AttributeBinaryMV].AddValue(UnitTestHelper.TestDataBinary2);
                Assert.AreEqual(1, resource.PendingChanges.Count);

                List<byte[]> expectedValues = new List<byte[]>();
                expectedValues.Add(UnitTestHelper.TestDataBinary2);

                this.AreByteArraysEqual(expectedValues, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                this.AreByteArraysEqual(expectedValues, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        private void AreByteArraysEqual(List<byte[]> expectedValues, ReadOnlyCollection<byte[]> actualValues)
        {
            Assert.AreEqual(expectedValues.Count, actualValues.Count);

            for (int i = 0; i < expectedValues.Count; i++)
            {
                CollectionAssert.AreEqual(expectedValues[i], actualValues[i]);
            }
        }

        [TestMethod]
        public void AddSecondBinaryMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeBinaryMV].AddValue(UnitTestHelper.TestDataBinary1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeBinaryMV].AddValue(UnitTestHelper.TestDataBinary2);
                Assert.AreEqual(1, resource.PendingChanges.Count);

                List<byte[]> expectedValues = new List<byte[]>();
                expectedValues.Add(UnitTestHelper.TestDataBinary1);
                expectedValues.Add(UnitTestHelper.TestDataBinary2);

                this.AreByteArraysEqual(expectedValues, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                this.AreByteArraysEqual(expectedValues, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues);
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
        public void ReplaceBinaryMV()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
                resource.Attributes[UnitTestHelper.AttributeBinaryMV].SetValue(UnitTestHelper.TestDataBinary1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[UnitTestHelper.AttributeBinaryMV].SetValue(UnitTestHelper.TestDataBinary2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                this.AreByteArraysEqual(UnitTestHelper.TestDataBinary2MV, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                this.AreByteArraysEqual(UnitTestHelper.TestDataBinary2MV, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues);
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
        public void DeleteFirstValueBinaryMV()
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
                resource.Attributes[UnitTestHelper.AttributeBinaryMV].RemoveValue(UnitTestHelper.TestDataBinary1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);

                List<byte[]> expectedValues = new List<byte[]>();
                expectedValues.Add(UnitTestHelper.TestDataBinary1MV[1]);
                expectedValues.Add(UnitTestHelper.TestDataBinary1MV[2]);

                this.AreByteArraysEqual(expectedValues, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                this.AreByteArraysEqual(expectedValues, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues);
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
        public void DeleteAllValueBinaryMV()
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
                resource.Attributes[UnitTestHelper.AttributeBinaryMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBinaryMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[UnitTestHelper.AttributeBinaryMV].IsNull);
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
