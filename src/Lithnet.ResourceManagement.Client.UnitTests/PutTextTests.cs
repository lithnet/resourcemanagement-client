using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class PutTextTests
    {
        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
        [DataRow(ConnectionMode.DirectNetTcp)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.DirectWsHttp)]
#endif
        public void AddTextSV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeTextSV].SetValue(Constants.TestDataText1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataText1, resource.Attributes[Constants.AttributeTextSV].StringValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(Constants.TestDataText1, resource.Attributes[Constants.AttributeTextSV].StringValue);
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
        public void ModifyTextSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeTextSV].SetValue(Constants.TestDataText1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeTextSV].SetValue(Constants.TestDataText2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.AreEqual(Constants.TestDataText2, resource.Attributes[Constants.AttributeTextSV].StringValue);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.AreEqual(Constants.TestDataText2, resource.Attributes[Constants.AttributeTextSV].StringValue);
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
        public void DeleteTextSV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeTextSV].SetValue(Constants.TestDataText1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeTextSV].RemoveValue(Constants.TestDataText1);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeTextSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                Assert.IsTrue(resource.Attributes[Constants.AttributeTextSV].IsNull);
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
        public void DeleteAllValueTextSV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeTextSV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeTextSV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[Constants.AttributeTextSV].IsNull);
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
        public void AddFirstTextMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeTextMV].AddValue(Constants.TestDataText2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[1] { Constants.TestDataText2 }, resource.Attributes[Constants.AttributeTextMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[1] { Constants.TestDataText2 }, resource.Attributes[Constants.AttributeTextMV].StringValues);
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
        public void AddSecondTextMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeTextMV].AddValue(Constants.TestDataText1);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeTextMV].AddValue(Constants.TestDataText2);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[2] { Constants.TestDataText1, Constants.TestDataText2 }, resource.Attributes[Constants.AttributeTextMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[2] { Constants.TestDataText1, Constants.TestDataText2 }, resource.Attributes[Constants.AttributeTextMV].StringValues);
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
        public void ReplaceTextMV(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = null;

            try
            {
                // Create the empty object
                resource = client.CreateResource(Constants.UnitTestObjectTypeName);
                resource.Attributes[Constants.AttributeTextMV].SetValue(Constants.TestDataText1MV);
                client.SaveResource(resource);

                // Re-get the object 
                resource = client.GetResource(resource.ObjectID);

                // Make the changes
                resource.Attributes[Constants.AttributeTextMV].SetValue(Constants.TestDataText2MV);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(Constants.TestDataText2MV, resource.Attributes[Constants.AttributeTextMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(Constants.TestDataText2MV, resource.Attributes[Constants.AttributeTextMV].StringValues);
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
        public void DeleteFirstValueTextMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeTextMV].RemoveValue(Constants.TestDataText1MV[0]);
                Assert.AreEqual(1, resource.PendingChanges.Count);
                CollectionAssert.AreEqual(new string[2] { Constants.TestDataText1MV[1], Constants.TestDataText1MV[2] }, resource.Attributes[Constants.AttributeTextMV].StringValues);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);

                CollectionAssert.AreEqual(new string[2] { Constants.TestDataText1MV[1], Constants.TestDataText1MV[2] }, resource.Attributes[Constants.AttributeTextMV].StringValues);
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
        public void DeleteAllValueTextMV(ConnectionMode connectionMode)
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
                resource.Attributes[Constants.AttributeTextMV].RemoveValues();
                Assert.AreEqual(1, resource.PendingChanges.Count);
                Assert.IsTrue(resource.Attributes[Constants.AttributeTextMV].IsNull);

                // Submit the changes
                client.SaveResource(resource);

                // Ensure there are no pending changes
                Assert.AreEqual(0, resource.PendingChanges.Count);

                resource = client.GetResource(resource.ObjectID);
                Assert.IsTrue(resource.Attributes[Constants.AttributeTextMV].IsNull);
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
