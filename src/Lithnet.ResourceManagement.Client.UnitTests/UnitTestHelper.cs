using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    internal static class UnitTestHelper
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        private static ResourceManagementClient client;


        internal static void PopulateTestUserData(ResourceObject resource)
        {
            PopulateTestUserData(resource, null);
        }

        internal static void PopulateTestUserData(ResourceObject resource, string accountName)
        {
            resource.Attributes[Constants.AttributeStringSV].SetValue(Constants.TestDataString1);
            resource.Attributes[Constants.AttributeStringMV].SetValue(Constants.TestDataString1MV);
            resource.Attributes[Constants.AttributeBinarySV].SetValue(Constants.TestDataBinary1);
            resource.Attributes[Constants.AttributeBinaryMV].SetValue(Constants.TestDataBinary1MV);
            resource.Attributes[Constants.AttributeBooleanSV].SetValue(Constants.TestDataBooleanTrue);
            resource.Attributes[Constants.AttributeDateTimeMV].SetValue(Constants.TestDataDateTime1MV);
            resource.Attributes[Constants.AttributeDateTimeSV].SetValue(Constants.TestDataDateTime1);
            resource.Attributes[Constants.AttributeIntegerMV].SetValue(Constants.TestDataInteger1MV);
            resource.Attributes[Constants.AttributeIntegerSV].SetValue(Constants.TestDataInteger1);
            resource.Attributes[Constants.AttributeReferenceMV].SetValue(Constants.TestDataReference1MV);
            resource.Attributes[Constants.AttributeReferenceSV].SetValue(Constants.TestDataReference1);
            resource.Attributes[Constants.AttributeTextMV].SetValue(Constants.TestDataText1MV);
            resource.Attributes[Constants.AttributeTextSV].SetValue(Constants.TestDataText1);

            if (accountName != null)
            {
                resource.Attributes[AttributeNames.AccountName].SetValue(accountName);
            }
        }

        internal static void ValidateTestUserData(ResourceObject resource)
        {
            // Validate single-valued attributes

            Assert.AreEqual(Constants.TestDataString1, resource.Attributes[Constants.AttributeStringSV].StringValue);
            Assert.AreEqual(Constants.TestDataBooleanTrue, resource.Attributes[Constants.AttributeBooleanSV].BooleanValue);
            Assert.AreEqual(Constants.TestDataDateTime1, resource.Attributes[Constants.AttributeDateTimeSV].DateTimeValue);
            Assert.AreEqual(Constants.TestDataInteger1, resource.Attributes[Constants.AttributeIntegerSV].IntegerValue);
            Assert.AreEqual(Constants.TestDataReference1, resource.Attributes[Constants.AttributeReferenceSV].ReferenceValue);
            Assert.AreEqual(Constants.TestDataText1, resource.Attributes[Constants.AttributeTextSV].StringValue);

            // Validate single-valued binary attribute

            CollectionAssert.AreEqual(Constants.TestDataBinary1, resource.Attributes[Constants.AttributeBinarySV].BinaryValue);

            // Validate multivalued attributes

            CollectionAssert.AreEqual(Constants.TestDataString1MV, resource.Attributes[Constants.AttributeStringMV].StringValues);
            CollectionAssert.AreEqual(Constants.TestDataDateTime1MV, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);
            CollectionAssert.AreEqual(Constants.TestDataInteger1MV, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);
            CollectionAssert.AreEqual(Constants.TestDataReference1MV, resource.Attributes[Constants.AttributeReferenceMV].ReferenceValues);
            CollectionAssert.AreEqual(Constants.TestDataText1MV, resource.Attributes[Constants.AttributeTextMV].StringValues);

            // Validate multivalued binary attribute

            Assert.AreEqual(Constants.TestDataBinary1MV.Count, resource.Attributes[Constants.AttributeBinaryMV].BinaryValues.Count);

            for (int i = 0; i < Constants.TestDataBinary1MV.Count; i++)
            {
                CollectionAssert.AreEqual(Constants.TestDataBinary1MV[i], resource.Attributes[Constants.AttributeBinaryMV].BinaryValues[i]);
            }
        }

        internal static void ValidateTestUserData(ResourceObject resource, List<string> attributesToCheck)
        {
            // Validate single-valued attributes

            if (attributesToCheck.Contains(Constants.AttributeStringSV))
            {
                Assert.AreEqual(Constants.TestDataString1, resource.Attributes[Constants.AttributeStringSV].StringValue);
            }

            if (attributesToCheck.Contains(Constants.AttributeBooleanSV))
            {
                Assert.AreEqual(Constants.TestDataBooleanTrue, resource.Attributes[Constants.AttributeBooleanSV].BooleanValue);
            }

            if (attributesToCheck.Contains(Constants.AttributeDateTimeSV))
            {
                Assert.AreEqual(Constants.TestDataDateTime1, resource.Attributes[Constants.AttributeDateTimeSV].DateTimeValue);
            }

            if (attributesToCheck.Contains(Constants.AttributeIntegerSV))
            {
                Assert.AreEqual(Constants.TestDataInteger1, resource.Attributes[Constants.AttributeIntegerSV].IntegerValue);
            }

            if (attributesToCheck.Contains(Constants.AttributeReferenceSV))
            {
                Assert.AreEqual(Constants.TestDataReference1, resource.Attributes[Constants.AttributeReferenceSV].ReferenceValue);
            }

            if (attributesToCheck.Contains(Constants.AttributeTextSV))
            {
                Assert.AreEqual(Constants.TestDataText1, resource.Attributes[Constants.AttributeTextSV].StringValue);
            }

            // Validate single-valued binary attribute

            if (attributesToCheck.Contains(Constants.AttributeBinarySV))
            {
                CollectionAssert.AreEqual(Constants.TestDataBinary1, resource.Attributes[Constants.AttributeBinarySV].BinaryValue);
            }

            // Validate multivalued attributes

            if (attributesToCheck.Contains(Constants.AttributeStringMV))
            {
                CollectionAssert.AreEqual(Constants.TestDataString1MV, resource.Attributes[Constants.AttributeStringMV].StringValues);
            }

            if (attributesToCheck.Contains(Constants.AttributeDateTimeMV))
            {
                CollectionAssert.AreEqual(Constants.TestDataDateTime1MV, resource.Attributes[Constants.AttributeDateTimeMV].DateTimeValues);
            }

            if (attributesToCheck.Contains(Constants.AttributeIntegerMV))
            {
                CollectionAssert.AreEqual(Constants.TestDataInteger1MV, resource.Attributes[Constants.AttributeIntegerMV].IntegerValues);
            }

            if (attributesToCheck.Contains(Constants.AttributeReferenceMV))
            {
                CollectionAssert.AreEqual(Constants.TestDataReference1MV, resource.Attributes[Constants.AttributeReferenceMV].ReferenceValues);
            }

            if (attributesToCheck.Contains(Constants.AttributeTextMV))
            {
                CollectionAssert.AreEqual(Constants.TestDataText1MV, resource.Attributes[Constants.AttributeTextMV].StringValues);
            }

            // Validate multivalued binary attribute

            if (attributesToCheck.Contains(Constants.AttributeBinaryMV))
            {
                Assert.AreEqual(Constants.TestDataBinary1MV.Count, resource.Attributes[Constants.AttributeBinaryMV].BinaryValues.Count);

                for (int i = 0; i < Constants.TestDataBinary1MV.Count; i++)
                {
                    CollectionAssert.AreEqual(Constants.TestDataBinary1MV[i], resource.Attributes[Constants.AttributeBinaryMV].BinaryValues[i]);
                }
            }
        }

        internal static ResourceObject CreateTestResource()
        {
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            resource.Save();
            return resource;
        }

        internal static ResourceObject CreateTestResource(string attributeName1, object value1, string attributeName2, object value2)
        {
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            resource.Attributes[attributeName1].SetValue(value1);
            resource.Attributes[attributeName2].SetValue(value2);
            resource.Save();
            return resource;
        }

        internal static ResourceObject CreateTestResource(string attributeName1, object value1)
        {
            ResourceObject resource = client.CreateResource(Constants.UnitTestObjectTypeName);
            resource.Attributes[attributeName1].SetValue(value1);
            resource.Save();
            return resource;
        }

        internal static void CleanupTestResources(params ResourceObject[] resources)
        {
            if (resources == null)
            {
                return;
            }

            foreach (ResourceObject resource in resources)
            {
                if (resource.ModificationType == OperationType.Update)
                {
                    client.DeleteResource(resource);
                }
            }
        }

        public static void ValidateSelectedAttributePresence(ResourceObject resource, List<string> attributes)
        {
            foreach (AttributeValue value in resource.Attributes)
            {
                if (attributes.Contains(value.AttributeName))
                {
                    Assert.IsFalse(value.IsNull);
                }
                else
                {
                    if (SchemaConstants.MandatoryAttributes.Contains(value.AttributeName))
                    {
                        continue;
                    }
                    else
                    {
                        Assert.IsTrue(value.IsNull);
                    }
                }
            }
        }

        internal static void Initialize(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            client = serviceProvider.GetRequiredService<ResourceManagementClient>();
        }
    }
}
