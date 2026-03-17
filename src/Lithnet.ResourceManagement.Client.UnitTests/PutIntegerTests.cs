using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{

    public class PutIntegerTests
    {
        [TestCaseSource(typeof(ConnectionModeSources))]
        public void AddIntegerSV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeIntegerSV].SetValue(Constants.TestDataInteger1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataInteger1, resource.Attributes[Constants.AttributeIntegerSV].IntegerValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(Constants.TestDataInteger1, resource.Attributes[Constants.AttributeIntegerSV].IntegerValue);
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
        public void ModifyIntegerSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeIntegerSV].SetValue(Constants.TestDataInteger1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeIntegerSV].SetValue(Constants.TestDataInteger2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataInteger2, resource.Attributes[Constants.AttributeIntegerSV].IntegerValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(Constants.TestDataInteger2, resource.Attributes[Constants.AttributeIntegerSV].IntegerValue);
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
        public void DeleteIntegerSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeIntegerSV].SetValue(Constants.TestDataInteger1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeIntegerSV].RemoveValue(Constants.TestDataInteger1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeIntegerSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[Constants.AttributeIntegerSV].IsNull);
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
        public void DeleteAllValueIntegerSV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeIntegerSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeIntegerSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[Constants.AttributeIntegerSV].IsNull);
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
        public void AddFirstIntegerMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeIntegerMV].AddValue(Constants.TestDataInteger2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new long[1] { Constants.TestDataInteger2 }, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new long[1] { Constants.TestDataInteger2 }, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);
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
        public void AddSecondIntegerMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeIntegerMV].AddValue(Constants.TestDataInteger1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeIntegerMV].AddValue(Constants.TestDataInteger2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new long[2] { Constants.TestDataInteger1, Constants.TestDataInteger2 }, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new long[2] { Constants.TestDataInteger1, Constants.TestDataInteger2 }, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);
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
        public void ReplaceIntegerMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeIntegerMV].SetValue(Constants.TestDataInteger1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeIntegerMV].SetValue(Constants.TestDataInteger2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(Constants.TestDataInteger2MV, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(Constants.TestDataInteger2MV, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);
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
        public void DeleteFirstValueIntegerMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeIntegerMV].RemoveValue(Constants.TestDataInteger1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new long[2] { Constants.TestDataInteger1MV[1], Constants.TestDataInteger1MV[2] }, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new long[2] { Constants.TestDataInteger1MV[1], Constants.TestDataInteger1MV[2] }, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);
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
        public void DeleteAllValueIntegerMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeIntegerMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeIntegerMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[Constants.AttributeIntegerMV].IsNull);
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
