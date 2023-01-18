using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    public static class UnitTestHelper
    {
        internal static ResourceManagementClient client = new ResourceManagementClient();

        internal const string TestLocale = "it-IT";
        internal static CultureInfo TestCulture = new CultureInfo(UnitTestHelper.TestLocale);

        public static string TestDataString1 = "testString1";
        public static string TestDataString2 = "testString2";
        public static string TestDataString3 = "testString3";

        public static long TestDataInteger1 = 4L;
        public static long TestDataInteger2 = 5L;
        public static long TestDataInteger3 = 6L;

        public static byte[] TestDataBinary1 = new byte[4] { 0, 1, 2, 3 };
        public static byte[] TestDataBinary2 = new byte[4] { 4, 5, 6, 7 };
        public static byte[] TestDataBinary3 = new byte[4] { 8, 9, 10, 11 };

        // We need to round-trip these values to avoid failed unit tests due to rounding
        public static DateTime TestDataDateTime1 = DateTime.Parse(DateTime.UtcNow.ToResourceManagementServiceDateFormat(true));
        public static DateTime TestDataDateTime2 = DateTime.Parse(DateTime.UtcNow.AddDays(1).ToResourceManagementServiceDateFormat(true));
        public static DateTime TestDataDateTime3 = DateTime.Parse(DateTime.UtcNow.AddDays(1).ToResourceManagementServiceDateFormat(true));

        public static UniqueIdentifier TestDataReference1;
        public static UniqueIdentifier TestDataReference2;
        public static UniqueIdentifier TestDataReference3;
        public static UniqueIdentifier TestDataReference4;
        public static UniqueIdentifier TestDataReference5;
        public static UniqueIdentifier TestDataReference6;

        public static string TestDataText1 = "testText1";
        public static string TestDataText2 = "testText2";
        public static string TestDataText3 = "testText3";

        public static bool TestDataBooleanTrue = true;
        public static bool TestDataBooleanFalse = false;

        public static List<string> TestDataString1MV = new List<string>() { "testString4", "testString5", "testString6" };
        public static List<string> TestDataString2MV = new List<string>() { "testString7", "testString8", "testString9" };

        public static List<long> TestDataInteger1MV = new List<long>() { 13L, 14L, 15L };
        public static List<long> TestDataInteger2MV = new List<long>() { 16L, 17L, 18L };

        public static List<byte[]> TestDataBinary1MV = new List<byte[]>() { new byte[4] { 12, 13, 14, 15 }, new byte[4] { 16, 17, 18, 19 }, new byte[4] { 20, 21, 22, 23 } };
        public static List<byte[]> TestDataBinary2MV = new List<byte[]>() { new byte[4] { 24, 25, 26, 27 }, new byte[4] { 28, 29, 30, 31 }, new byte[4] { 32, 33, 34, 35 } };

        public static List<DateTime> TestDataDateTime1MV = new List<DateTime>() {
            DateTime.Parse(DateTime.UtcNow.AddDays(3).ToResourceManagementServiceDateFormat(true)),
            DateTime.Parse(DateTime.UtcNow.AddDays(4).ToResourceManagementServiceDateFormat(true)),
            DateTime.Parse(DateTime.UtcNow.AddDays(5).ToResourceManagementServiceDateFormat(true)) };

        public static List<DateTime> TestDataDateTime2MV = new List<DateTime>() {
            DateTime.Parse(DateTime.UtcNow.AddDays(6).ToResourceManagementServiceDateFormat(true)),
            DateTime.Parse(DateTime.UtcNow.AddDays(7).ToResourceManagementServiceDateFormat(true)),
            DateTime.Parse(DateTime.UtcNow.AddDays(8).ToResourceManagementServiceDateFormat(true)) };

        public static List<UniqueIdentifier> TestDataReference1MV;
        public static List<UniqueIdentifier> TestDataReference2MV;

        public static List<string> TestDataText1MV = new List<string>() { "testText4", "testText5", "testText6" };
        public static List<string> TestDataText2MV = new List<string>() { "testText7", "testText8", "testText9" };

        internal const string ObjectTypeUnitTestObjectName = "_unitTestObject";
        internal const string AttributeStringSV = "ut_svstring";
        internal const string AttributeStringMV = "ut_mvstring";
        internal const string AttributeIntegerSV = "ut_svinteger";
        internal const string AttributeIntegerMV = "ut_mvinteger";
        internal const string AttributeReferenceSV = "ut_svreference";
        internal const string AttributeReferenceMV = "ut_mvreference";
        internal const string AttributeTextSV = "ut_svtext";
        internal const string AttributeTextMV = "ut_mvtext";
        internal const string AttributeDateTimeSV = "ut_svdatetime";
        internal const string AttributeDateTimeMV = "ut_mvdatetime";
        internal const string AttributeBinarySV = "ut_svbinary";
        internal const string AttributeBinaryMV = "ut_mvbinary";
        internal const string AttributeBooleanSV = "ut_svboolean";

        static UnitTestHelper()
        {
            UnitTestHelper.PrepareRMSForUnitTests();
            UnitTestHelper.CreateReferenceTestObjects();
        }

        private static void CreateReferenceTestObjects()
        {
            UnitTestHelper.TestDataReference1 = UnitTestHelper.CreateReferenceTestObjectIfDoesntExist("reftest1").ObjectID;
            UnitTestHelper.TestDataReference2 = UnitTestHelper.CreateReferenceTestObjectIfDoesntExist("reftest2").ObjectID;
            UnitTestHelper.TestDataReference3 = UnitTestHelper.CreateReferenceTestObjectIfDoesntExist("reftest3").ObjectID;
            UnitTestHelper.TestDataReference4 = UnitTestHelper.CreateReferenceTestObjectIfDoesntExist("reftest4").ObjectID;
            UnitTestHelper.TestDataReference5 = UnitTestHelper.CreateReferenceTestObjectIfDoesntExist("reftest5").ObjectID;
            UnitTestHelper.TestDataReference6 = UnitTestHelper.CreateReferenceTestObjectIfDoesntExist("reftest6").ObjectID;

            UnitTestHelper.TestDataReference1MV = new List<UniqueIdentifier>() { TestDataReference1, TestDataReference2, TestDataReference3 };
            UnitTestHelper.TestDataReference2MV = new List<UniqueIdentifier>() { TestDataReference4, TestDataReference5, TestDataReference6 };
        }

        private static ResourceObject CreateReferenceTestObjectIfDoesntExist(string accountName)
        {
            ResourceObject testObject = client.GetResourceByKey(UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName, accountName);

            if (testObject != null)
            {
                return testObject;
            }

            testObject = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            testObject.Attributes[AttributeNames.AccountName].SetValue( accountName);

            client.SaveResource(testObject);

            return testObject;
        }

        public static void PrepareRMSForUnitTests()
        {
            ResourceObject objectClass = CreateUnitTestObjectTypeIfDoesntExist();

            ResourceObject svStringAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeStringSV, null, false, AttributeType.String);
            ResourceObject mvStringAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeStringMV, null, true, AttributeType.String);
            ResourceObject svIntegerAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeIntegerSV, null, false, AttributeType.Integer);
            ResourceObject mvIntegerAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeIntegerMV, null, true, AttributeType.Integer);
            ResourceObject svReferenceAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeReferenceSV, null, false, AttributeType.Reference);
            ResourceObject mvReferenceAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeReferenceMV, null, true, AttributeType.Reference);
            ResourceObject svTextAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeTextSV, null, false, AttributeType.Text);
            ResourceObject mvTextAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeTextMV, null, true, AttributeType.Text);
            ResourceObject svDateTimeAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeDateTimeSV, null, false, AttributeType.DateTime);
            ResourceObject mvDateTimeAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeDateTimeMV, null, true, AttributeType.DateTime);
            ResourceObject svBinaryAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeBinarySV, null, false, AttributeType.Binary);
            ResourceObject mvBinaryAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeBinaryMV, null, true, AttributeType.Binary);
            ResourceObject svBooleanAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeBooleanSV, null, false, AttributeType.Boolean);
            ResourceObject accountNameAttribute = UnitTestHelper.CreateAttributeTypeIfDoesntExist(AttributeNames.AccountName, null, false, AttributeType.String);

            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, svStringAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, mvStringAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, svIntegerAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, mvIntegerAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, svReferenceAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, mvReferenceAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, svTextAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, mvTextAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, svDateTimeAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, mvDateTimeAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, svBinaryAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, mvBinaryAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, svBooleanAttribute);
            UnitTestHelper.CreateBindingIfDoesntExist(objectClass, accountNameAttribute);

            client.RefreshSchema();
        }

        private static ResourceObject CreateUnitTestObjectTypeIfDoesntExist()
        {
            ResourceObject testObject = client.GetResourceByKey(ObjectTypeNames.ObjectTypeDescription, AttributeNames.Name, ObjectTypeUnitTestObjectName);

            if (testObject != null)
            {
                return testObject;
            }

            testObject = client.CreateResource(ObjectTypeNames.ObjectTypeDescription);
            testObject.Attributes[AttributeNames.Name].SetValue( UnitTestHelper.ObjectTypeUnitTestObjectName);
            testObject.Attributes[AttributeNames.DisplayName].SetValue( UnitTestHelper.ObjectTypeUnitTestObjectName);

            client.SaveResource(testObject);

            return testObject;
        }

        private static ResourceObject CreateAttributeTypeIfDoesntExist(string typeName, string regex, bool multivalued, AttributeType type)
        {
            ResourceObject testAttribute = client.GetResourceByKey(ObjectTypeNames.AttributeTypeDescription, AttributeNames.Name, typeName);

            if (testAttribute != null)
            {
                return testAttribute;
            }

            testAttribute = client.CreateResource(ObjectTypeNames.AttributeTypeDescription);
            testAttribute.Attributes[AttributeNames.Name].SetValue(typeName);
            testAttribute.Attributes[AttributeNames.DisplayName].SetValue( typeName);

            if (regex != null)
            {
                testAttribute.Attributes[AttributeNames.StringRegex].SetValue( regex);
            }

            testAttribute.Attributes[AttributeNames.Multivalued].SetValue( multivalued);
            testAttribute.Attributes[AttributeNames.DataType].SetValue( type.ToString());

            client.SaveResource(testAttribute);

            return testAttribute;
        }

        private static void CreateBindingIfDoesntExist(ResourceObject objectType, ResourceObject attributeType)
        {
            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add(AttributeNames.BoundObjectType, objectType.ObjectID.Value);
            keys.Add(AttributeNames.BoundAttributeType, attributeType.ObjectID.Value);

            ResourceObject testObject = client.GetResourceByKey(ObjectTypeNames.BindingDescription, keys);

            if (testObject != null)
            {
                return;
            }

            ResourceObject resource = client.CreateResource(ObjectTypeNames.BindingDescription);
            resource.Attributes[AttributeNames.BoundObjectType].SetValue( objectType.ObjectID);
            resource.Attributes[AttributeNames.BoundAttributeType].SetValue( attributeType.ObjectID);
            resource.Attributes[AttributeNames.Required].SetValue( false);

            client.SaveResource(resource);
        }

        internal static void PopulateTestUserData(ResourceObject resource)
        {
            PopulateTestUserData(resource, null);
        }

        internal static void PopulateTestUserData(ResourceObject resource, string accountName)
        {
            resource.Attributes[UnitTestHelper.AttributeStringSV].SetValue(UnitTestHelper.TestDataString1);
            resource.Attributes[UnitTestHelper.AttributeStringMV].SetValue(UnitTestHelper.TestDataString1MV);
            resource.Attributes[UnitTestHelper.AttributeBinarySV].SetValue( UnitTestHelper.TestDataBinary1);
            resource.Attributes[UnitTestHelper.AttributeBinaryMV].SetValue( UnitTestHelper.TestDataBinary1MV);
            resource.Attributes[UnitTestHelper.AttributeBooleanSV].SetValue( UnitTestHelper.TestDataBooleanTrue);
            resource.Attributes[UnitTestHelper.AttributeDateTimeMV].SetValue( UnitTestHelper.TestDataDateTime1MV);
            resource.Attributes[UnitTestHelper.AttributeDateTimeSV].SetValue( UnitTestHelper.TestDataDateTime1);
            resource.Attributes[UnitTestHelper.AttributeIntegerMV].SetValue( UnitTestHelper.TestDataInteger1MV);
            resource.Attributes[UnitTestHelper.AttributeIntegerSV].SetValue( UnitTestHelper.TestDataInteger1);
            resource.Attributes[UnitTestHelper.AttributeReferenceMV].SetValue( UnitTestHelper.TestDataReference1MV);
            resource.Attributes[UnitTestHelper.AttributeReferenceSV].SetValue( UnitTestHelper.TestDataReference1);
            resource.Attributes[UnitTestHelper.AttributeTextMV].SetValue( UnitTestHelper.TestDataText1MV);
            resource.Attributes[UnitTestHelper.AttributeTextSV].SetValue( UnitTestHelper.TestDataText1);

            if (accountName != null)
            {
                resource.Attributes[AttributeNames.AccountName].SetValue(accountName);
            }
        }

        internal static void ValidateTestUserData(ResourceObject resource)
        {
            // Validate single-valued attributes

            Assert.AreEqual(UnitTestHelper.TestDataString1, resource.Attributes[UnitTestHelper.AttributeStringSV].StringValue);
            Assert.AreEqual(UnitTestHelper.TestDataBooleanTrue, resource.Attributes[UnitTestHelper.AttributeBooleanSV].BooleanValue);
            Assert.AreEqual(UnitTestHelper.TestDataDateTime1, resource.Attributes[UnitTestHelper.AttributeDateTimeSV].DateTimeValue);
            Assert.AreEqual(UnitTestHelper.TestDataInteger1, resource.Attributes[UnitTestHelper.AttributeIntegerSV].IntegerValue);
            Assert.AreEqual(UnitTestHelper.TestDataReference1, resource.Attributes[UnitTestHelper.AttributeReferenceSV].ReferenceValue);
            Assert.AreEqual(UnitTestHelper.TestDataText1, resource.Attributes[UnitTestHelper.AttributeTextSV].StringValue);

            // Validate single-valued binary attribute

            CollectionAssert.AreEqual(UnitTestHelper.TestDataBinary1, resource.Attributes[UnitTestHelper.AttributeBinarySV].BinaryValue);

            // Validate multivalued attributes

            CollectionAssert.AreEqual(UnitTestHelper.TestDataString1MV, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);
            CollectionAssert.AreEqual(UnitTestHelper.TestDataDateTime1MV, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);
            CollectionAssert.AreEqual(UnitTestHelper.TestDataInteger1MV, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);
            CollectionAssert.AreEqual(UnitTestHelper.TestDataReference1MV, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);
            CollectionAssert.AreEqual(UnitTestHelper.TestDataText1MV, resource.Attributes[UnitTestHelper.AttributeTextMV].StringValues);

            // Validate multivalued binary attribute

            Assert.AreEqual(UnitTestHelper.TestDataBinary1MV.Count, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues.Count);

            for (int i = 0; i < UnitTestHelper.TestDataBinary1MV.Count; i++)
            {
                CollectionAssert.AreEqual(UnitTestHelper.TestDataBinary1MV[i], resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues[i]);
            }
        }

        internal static void ValidateTestUserData(ResourceObject resource, List<string> attributesToCheck)
        {
            // Validate single-valued attributes

            if (attributesToCheck.Contains(UnitTestHelper.AttributeStringSV))
            {
                Assert.AreEqual(UnitTestHelper.TestDataString1, resource.Attributes[UnitTestHelper.AttributeStringSV].StringValue);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeBooleanSV))
            {
                Assert.AreEqual(UnitTestHelper.TestDataBooleanTrue, resource.Attributes[UnitTestHelper.AttributeBooleanSV].BooleanValue);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeDateTimeSV))
            {
                Assert.AreEqual(UnitTestHelper.TestDataDateTime1, resource.Attributes[UnitTestHelper.AttributeDateTimeSV].DateTimeValue);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeIntegerSV))
            {
                Assert.AreEqual(UnitTestHelper.TestDataInteger1, resource.Attributes[UnitTestHelper.AttributeIntegerSV].IntegerValue);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeReferenceSV))
            {
                Assert.AreEqual(UnitTestHelper.TestDataReference1, resource.Attributes[UnitTestHelper.AttributeReferenceSV].ReferenceValue);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeTextSV))
            {
                Assert.AreEqual(UnitTestHelper.TestDataText1, resource.Attributes[UnitTestHelper.AttributeTextSV].StringValue);
            }

            // Validate single-valued binary attribute

            if (attributesToCheck.Contains(UnitTestHelper.AttributeBinarySV))
            {
                CollectionAssert.AreEqual(UnitTestHelper.TestDataBinary1, resource.Attributes[UnitTestHelper.AttributeBinarySV].BinaryValue);
            }

            // Validate multivalued attributes

            if (attributesToCheck.Contains(UnitTestHelper.AttributeStringMV))
            {
                CollectionAssert.AreEqual(UnitTestHelper.TestDataString1MV, resource.Attributes[UnitTestHelper.AttributeStringMV].StringValues);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeDateTimeMV))
            {
                CollectionAssert.AreEqual(UnitTestHelper.TestDataDateTime1MV, resource.Attributes[UnitTestHelper.AttributeDateTimeMV].DateTimeValues);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeIntegerMV))
            {
                CollectionAssert.AreEqual(UnitTestHelper.TestDataInteger1MV, resource.Attributes[UnitTestHelper.AttributeIntegerMV].IntegerValues);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeReferenceMV))
            {
                CollectionAssert.AreEqual(UnitTestHelper.TestDataReference1MV, resource.Attributes[UnitTestHelper.AttributeReferenceMV].ReferenceValues);
            }

            if (attributesToCheck.Contains(UnitTestHelper.AttributeTextMV))
            {
                CollectionAssert.AreEqual(UnitTestHelper.TestDataText1MV, resource.Attributes[UnitTestHelper.AttributeTextMV].StringValues);
            }

            // Validate multivalued binary attribute

            if (attributesToCheck.Contains(UnitTestHelper.AttributeBinaryMV))
            {
                Assert.AreEqual(UnitTestHelper.TestDataBinary1MV.Count, resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues.Count);

                for (int i = 0; i < UnitTestHelper.TestDataBinary1MV.Count; i++)
                {
                    CollectionAssert.AreEqual(UnitTestHelper.TestDataBinary1MV[i], resource.Attributes[UnitTestHelper.AttributeBinaryMV].BinaryValues[i]);
                }
            }
        }

        internal static ResourceObject CreateTestResource()
        {
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Save();
            return resource;
        }

        internal static ResourceObject CreateTestResource(string attributeName1, object value1, string attributeName2, object value2)
        {
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
            resource.Attributes[attributeName1].SetValue(value1);
            resource.Attributes[attributeName2].SetValue(value2);
            resource.Save();
            return resource;
        }

        internal static ResourceObject CreateTestResource(string attributeName1, object value1)
        {
            ResourceObject resource = client.CreateResource(UnitTestHelper.ObjectTypeUnitTestObjectName);
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
    }
}
