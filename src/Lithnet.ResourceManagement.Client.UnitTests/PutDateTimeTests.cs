using System;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{

    public class PutDateTimeTests
    {
        [TestCaseSource(typeof(ConnectionModeSources))]
        public void AddDateTimeSV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeDateTimeSV].SetValue(Constants.TestDataDateTime1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataDateTime1, resource.Attributes[Constants.AttributeDateTimeSV].DateTimeValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(Constants.TestDataDateTime1, resource.Attributes[Constants.AttributeDateTimeSV].DateTimeValue);
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
        public void ModifyDateTimeSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeDateTimeSV].SetValue(Constants.TestDataDateTime1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeDateTimeSV].SetValue(Constants.TestDataDateTime2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataDateTime2, resource.Attributes[Constants.AttributeDateTimeSV].DateTimeValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(Constants.TestDataDateTime2, resource.Attributes[Constants.AttributeDateTimeSV].DateTimeValue);
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
        public void DeleteDateTimeSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeDateTimeSV].SetValue(Constants.TestDataDateTime1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeDateTimeSV].RemoveValue(Constants.TestDataDateTime1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeDateTimeSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[Constants.AttributeDateTimeSV].IsNull);
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
        public void DeleteAllValueDateTimeSV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeDateTimeSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeDateTimeSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[Constants.AttributeDateTimeSV].IsNull);
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
        public void AddFirstDateTimeMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeDateTimeMV].AddValue(Constants.TestDataDateTime2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new DateTime[1] { Constants.TestDataDateTime2 }, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new DateTime[1] { Constants.TestDataDateTime2 }, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);
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
        public void AddSecondDateTimeMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeDateTimeMV].AddValue(Constants.TestDataDateTime1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeDateTimeMV].AddValue(Constants.TestDataDateTime2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new DateTime[2] { Constants.TestDataDateTime1, Constants.TestDataDateTime2 }, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new DateTime[2] { Constants.TestDataDateTime1, Constants.TestDataDateTime2 }, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);
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
        public void ReplaceDateTimeMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeDateTimeMV].SetValue(Constants.TestDataDateTime1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeDateTimeMV].SetValue(Constants.TestDataDateTime2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(Constants.TestDataDateTime2MV, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(Constants.TestDataDateTime2MV, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);
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
        public void DeleteFirstValueDateTimeMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeDateTimeMV].RemoveValue(Constants.TestDataDateTime1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new DateTime[2] { Constants.TestDataDateTime1MV[1], Constants.TestDataDateTime1MV[2] }, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new DateTime[2] { Constants.TestDataDateTime1MV[1], Constants.TestDataDateTime1MV[2] }, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);
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
        public void DeleteAllValueDateTimeMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeDateTimeMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeDateTimeMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[Constants.AttributeDateTimeMV].IsNull);
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
