using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{

    public class PutStringTests
    {
        [TestCaseSource(typeof(ConnectionModeSources))]
        public void AddStringSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataString1, resource.Attributes[Constants.AttributeStringSV].StringValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(Constants.TestDataString1, resource.Attributes[Constants.AttributeStringSV].StringValue);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void ModifyStringSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataString2, resource.Attributes[Constants.AttributeStringSV].StringValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(Constants.TestDataString2, resource.Attributes[Constants.AttributeStringSV].StringValue);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void ModifyStringSVNoUpdate(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString1);
                Assert.AreEqual(0, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataString1, resource.Attributes[Constants.AttributeStringSV].StringValue);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteStringSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringSV].RemoveValue(Constants.TestDataString1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeStringSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[Constants.AttributeStringSV].IsNull);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteAllValueStringSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeStringSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[Constants.AttributeStringSV].IsNull);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void AddFirstStringMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringMV].AddValue(Constants.TestDataString2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[1] { Constants.TestDataString2 }, resource.Attributes[Constants.AttributeStringMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[1] { Constants.TestDataString2 }, resource.Attributes[Constants.AttributeStringMV].StringValues);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void AddSecondStringMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeStringMV].AddValue(Constants.TestDataString1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringMV].AddValue(Constants.TestDataString2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[2] { Constants.TestDataString1, Constants.TestDataString2 }, resource.Attributes[Constants.AttributeStringMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[2] { Constants.TestDataString1, Constants.TestDataString2 }, resource.Attributes[Constants.AttributeStringMV].StringValues);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void ReplaceStringMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeStringMV].SetValue(Constants.TestDataString1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringMV].SetValue(Constants.TestDataString2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(Constants.TestDataString2MV, resource.Attributes[Constants.AttributeStringMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(Constants.TestDataString2MV, resource.Attributes[Constants.AttributeStringMV].StringValues);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteFirstValueStringMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringMV].RemoveValue(Constants.TestDataString1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[2] { Constants.TestDataString1MV[1], Constants.TestDataString1MV[2] }, resource.Attributes[Constants.AttributeStringMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[2] { Constants.TestDataString1MV[1], Constants.TestDataString1MV[2] }, resource.Attributes[Constants.AttributeStringMV].StringValues);
            }
            finally
            {
                if (resource != null && !resource.IsPlaceHolder)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        [TestCaseSource(typeof(ConnectionModeSources))]
        public void DeleteAllValueStringMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                UnitTestHelper.PopulateTestUserData(resource);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeStringMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeStringMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[Constants.AttributeStringMV].IsNull);
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