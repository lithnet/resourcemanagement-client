using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class GetTests
    {
        [TestMethod]
        public void GetObjectByIDWithAllAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);

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

        [TestMethod]
        public void GetObjectByIDStringWithAllAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);

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

        [TestMethod]
        public void GetObjectByIDGuidWithAllAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);

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

        [TestMethod]
        public void GetObjectByIDWithSelectedAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            List<string> attributesToGet = new List<string>() { UnitTestHelper.AttributeBooleanSV, UnitTestHelper.AttributeStringSV, UnitTestHelper.AttributeReferenceMV };

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

        [TestMethod]
        public void GetObjectByIDStringWithSelectedAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            List<string> attributesToGet = new List<string>() { UnitTestHelper.AttributeBooleanSV, UnitTestHelper.AttributeStringSV, UnitTestHelper.AttributeReferenceMV };

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

        [TestMethod]
        public void GetObjectByIDGuidWithSelectedAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            List<string> attributesToGet = new List<string>() { UnitTestHelper.AttributeBooleanSV, UnitTestHelper.AttributeStringSV, UnitTestHelper.AttributeReferenceMV };

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

        [TestMethod]
        public void GetObjectByKeyWithSelectedAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            string newID = Guid.NewGuid().ToString();
            List<string> attributesToGet = new List<string>() { AttributeNames.AccountName, UnitTestHelper.AttributeBooleanSV, UnitTestHelper.AttributeStringSV, UnitTestHelper.AttributeReferenceMV };

            try
            {
                UnitTestHelper.PopulateTestUserData(resource, newID);
                resource.Save();

                resource = client.GetResourceByKey(UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName, newID, attributesToGet);
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

        [TestMethod]
        public void GetObjectByKeyWithAllAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            string newID = Guid.NewGuid().ToString();

            try
            {
                UnitTestHelper.PopulateTestUserData(resource, newID);
                resource.Save();

                resource = client.GetResourceByKey(UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName, newID);

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

        [TestMethod]
        public void GetObjectByMultipleKeysWithSelectedAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            string newID = Guid.NewGuid().ToString();
            List<string> attributesToGet = new List<string>() { AttributeNames.AccountName, UnitTestHelper.AttributeBooleanSV, UnitTestHelper.AttributeStringSV, UnitTestHelper.AttributeReferenceMV };

            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add(AttributeNames.AccountName, newID);
            keys.Add(UnitTestHelper.AttributeStringSV, UnitTestHelper.TestDataString1);

            try
            {
                UnitTestHelper.PopulateTestUserData(resource, newID);
                resource.Save();

                resource = client.GetResourceByKey(UnitTestHelper.ObjectTypeUnitTestObjectName, keys, attributesToGet);
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

        [TestMethod]
        public void GetObjectByMultipleKeysWithAllAttributes()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            string newID = Guid.NewGuid().ToString();
            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add(AttributeNames.AccountName, newID);
            keys.Add(UnitTestHelper.AttributeStringSV, UnitTestHelper.TestDataString1);

            try
            {
                UnitTestHelper.PopulateTestUserData(resource, newID);
                resource.Save();

                resource = client.GetResourceByKey(UnitTestHelper.ObjectTypeUnitTestObjectName, keys);
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

        [TestMethod]
        public void GetObjectByKeyFailsOnMultipleResults()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            ResourceObject resource1 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            ResourceObject resource2 = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            string newID = Guid.NewGuid().ToString();

            try
            {
                UnitTestHelper.PopulateTestUserData(resource1);
                resource1.Save();

                UnitTestHelper.PopulateTestUserData(resource2);
                resource2.Save();

                try
                {
                    resource1 = client.GetResourceByKey(UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, UnitTestHelper.TestDataString1);
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

        [TestMethod]
        public void GetObjectByKeyReturnsNullOnNoResults()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            ResourceObject resource1 = client.GetResourceByKey(UnitTestHelper.ObjectTypeUnitTestObjectName, UnitTestHelper.AttributeStringSV, Guid.NewGuid().ToString());
            Assert.IsNull(resource1);
        }
    }
}
