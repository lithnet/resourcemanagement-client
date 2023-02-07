using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class GetTests
    {
        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByIDWithAllAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);

            try
            {
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();
                resource = client.GetResource(resource.ObjectID);

                UnitTestHelper.ValidateTestUserData(resource);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByIDStringWithAllAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);

            try
            {
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();

                resource = client.GetResource(resource.ObjectID.Value);

                UnitTestHelper.ValidateTestUserData(resource);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByIDGuidWithAllAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);

            try
            {
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();

                resource = client.GetResource(resource.ObjectID.GetGuid());

                UnitTestHelper.ValidateTestUserData(resource);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByIDWithSelectedAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            List<string> attributesToGet = new List<string>() { Constants.AttributeBooleanSV, Constants.AttributeStringSV, Constants.AttributeReferenceMV };

            try
            {
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();

                resource = client.GetResource(resource.ObjectID, attributesToGet);
                UnitTestHelper.ValidateSelectedAttributePresence(resource, attributesToGet);
                UnitTestHelper.ValidateTestUserData(resource, attributesToGet);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByIDStringWithSelectedAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            List<string> attributesToGet = new List<string>() { Constants.AttributeBooleanSV, Constants.AttributeStringSV, Constants.AttributeReferenceMV };

            try
            {
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();

                resource = client.GetResource(resource.ObjectID.Value, attributesToGet);
                UnitTestHelper.ValidateSelectedAttributePresence(resource, attributesToGet);
                UnitTestHelper.ValidateTestUserData(resource, attributesToGet);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByIDGuidWithSelectedAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            List<string> attributesToGet = new List<string>() { Constants.AttributeBooleanSV, Constants.AttributeStringSV, Constants.AttributeReferenceMV };

            try
            {
                UnitTestHelper.PopulateTestUserData(resource);
                resource.Save();

                resource = client.GetResource(resource.ObjectID.GetGuid(), attributesToGet);
                UnitTestHelper.ValidateSelectedAttributePresence(resource, attributesToGet);
                UnitTestHelper.ValidateTestUserData(resource, attributesToGet);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByKeyWithSelectedAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            string newID = Guid.NewGuid().ToString();
            List<string> attributesToGet = new List<string>() { AttributeNames.AccountName, Constants.AttributeBooleanSV, Constants.AttributeStringSV, Constants.AttributeReferenceMV };

            try
            {
                UnitTestHelper.PopulateTestUserData(resource, newID);
                resource.Save();

                resource = client.GetResourceByKey(Constants.UnitTestObjectTypeName, AttributeNames.AccountName, newID, attributesToGet);
                UnitTestHelper.ValidateSelectedAttributePresence(resource, attributesToGet);
                UnitTestHelper.ValidateTestUserData(resource, attributesToGet);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByKeyWithAllAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            string newID = Guid.NewGuid().ToString();

            try
            {
                UnitTestHelper.PopulateTestUserData(resource, newID);
                resource.Save();

                resource = client.GetResourceByKey(Constants.UnitTestObjectTypeName, AttributeNames.AccountName, newID);

                UnitTestHelper.ValidateTestUserData(resource);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByMultipleKeysWithSelectedAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            string newID = Guid.NewGuid().ToString();
            List<string> attributesToGet = new List<string>() { AttributeNames.AccountName, Constants.AttributeBooleanSV, Constants.AttributeStringSV, Constants.AttributeReferenceMV };

            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add(AttributeNames.AccountName, newID);
            keys.Add(Constants.AttributeStringSV, Constants.TestDataString1);

            try
            {
                UnitTestHelper.PopulateTestUserData(resource, newID);
                resource.Save();

                resource = client.GetResourceByKey(Constants.UnitTestObjectTypeName, keys, attributesToGet);
                UnitTestHelper.ValidateSelectedAttributePresence(resource, attributesToGet);
                UnitTestHelper.ValidateTestUserData(resource, attributesToGet);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByMultipleKeysWithAllAttributes(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            string newID = Guid.NewGuid().ToString();
            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add(AttributeNames.AccountName, newID);
            keys.Add(Constants.AttributeStringSV, Constants.TestDataString1);

            try
            {
                UnitTestHelper.PopulateTestUserData(resource, newID);
                resource.Save();

                resource = client.GetResourceByKey(Constants.UnitTestObjectTypeName, keys);
                UnitTestHelper.ValidateTestUserData(resource);
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
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByKeyFailsOnMultipleResults(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);
            ResourceObject resource1 = client.CreateResource(Constants.UnitTestObjectTypeName);
            ResourceObject resource2 = client.CreateResource(Constants.UnitTestObjectTypeName);
            string newID = Guid.NewGuid().ToString();

            try
            {
                UnitTestHelper.PopulateTestUserData(resource1);
                resource1.Save();

                UnitTestHelper.PopulateTestUserData(resource2);
                resource2.Save();

                try
                {
                    resource1 = client.GetResourceByKey(Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, Constants.TestDataString1);
                    Assert.Fail("The expectedXpath exception was not thrown");
                }
                catch (TooManyResultsException)
                {
                }
            }
            finally
            {
                if (resource1 != null && !resource1.IsPlaceHolder)
                {
                    client.DeleteResource(resource1);
                }

                if (resource2 != null && !resource2.IsPlaceHolder)
                {
                    client.DeleteResource(resource2);
                }
            }
        }

        [DataTestMethod]
        [DataRow(ConnectionMode.RemoteProxy)]
        [DataRow(ConnectionMode.LocalProxy)]
#if NETFRAMEWORK

        [DataRow(ConnectionMode.Direct)]
#endif
        public void GetObjectByKeyReturnsNullOnNoResults(ConnectionMode connectionMode)
        {
            var client = UnitTestHelper.GetClient(connectionMode);

            ResourceObject resource1 = client.GetResourceByKey(Constants.UnitTestObjectTypeName, Constants.AttributeStringSV, Guid.NewGuid().ToString());
            Assert.IsNull(resource1);
        }
    }
}
