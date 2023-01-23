using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    internal class Setup
    {
        static ResourceManagementClient client;

        [AssemblyInitialize]
        public static void InitializeTestEngine(TestContext testContext)
        {
            Init();
            PrepareRMSForUnitTests();
            CreateReferenceTestObjects();
        }

        private static void Init()
        {
            var builder = new HostBuilder();
            builder.ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
            {
                configurationBuilder.AddJsonFile($"appsettings.json", optional: false);
                configurationBuilder.AddEnvironmentVariables("LithnetRMC");
            });

            // if you want to override Physical database with in-memory database
            builder.ConfigureServices((context, services) =>
            {
                services.AddOptions<ResourceManagementClientOptions>();

                services.Configure<ResourceManagementClientOptions>(context.Configuration.GetSection("LithnetResourceManagementClient"));
                services.AddSingleton<ResourceManagementClient>();
                services.PostConfigure<ResourceManagementClientOptions>((op) =>
                {
                    if (!FrameworkUtilities.IsFramework)
                    {
                        return;
                    }

                    var op2 = ClientConfigurationSection.GetOptionsFromConfiguration();
                    if (op2 != null)
                    {
                        op.BaseUri = op2.BaseUri;
                        op.Spn = op2.Spn;
                        op.ConcurrentConnectionLimit = op2.ConcurrentConnectionLimit;
                        op.ConnectTimeoutSeconds = op2.ConnectTimeoutSeconds;
                        op.SendTimeoutSeconds = op2.SendTimeoutSeconds;
                        op.RecieveTimeoutSeconds = op2.RecieveTimeoutSeconds;
                        op.Password = op2.Password;
                        op.Username = op2.Username;
                    }
                });
            });

            var host = builder.Build();
            UnitTestHelper.Initialize(host.Services);
            client = host.Services.GetRequiredService<ResourceManagementClient>();
        }

        private static void CreateReferenceTestObjects()
        {
            Constants.TestDataReference1 = CreateReferenceTestObjectIfDoesntExist("reftest1").ObjectID;
            Constants.TestDataReference2 = CreateReferenceTestObjectIfDoesntExist("reftest2").ObjectID;
            Constants.TestDataReference3 = CreateReferenceTestObjectIfDoesntExist("reftest3").ObjectID;
            Constants.TestDataReference4 = CreateReferenceTestObjectIfDoesntExist("reftest4").ObjectID;
            Constants.TestDataReference5 = CreateReferenceTestObjectIfDoesntExist("reftest5").ObjectID;
            Constants.TestDataReference6 = CreateReferenceTestObjectIfDoesntExist("reftest6").ObjectID;

            Constants.TestDataReference1MV = new List<UniqueIdentifier>() { Constants.TestDataReference1, Constants.TestDataReference2, Constants.TestDataReference3 };
            Constants.TestDataReference2MV = new List<UniqueIdentifier>() { Constants.TestDataReference4, Constants.TestDataReference5, Constants.TestDataReference6 };
        }

        private static ResourceObject CreateReferenceTestObjectIfDoesntExist(string accountName)
        {
            ResourceObject testObject = client.GetResourceByKey(Constants.UnitTestObjectTypeName, AttributeNames.AccountName, accountName);

            if (testObject != null)
            {
                return testObject;
            }

            testObject = client.CreateResource(Constants.UnitTestObjectTypeName);
            testObject.Attributes[AttributeNames.AccountName].SetValue(accountName);

            client.SaveResource(testObject);

            return testObject;
        }

        public static void PrepareRMSForUnitTests()
        {
            ResourceObject objectClass = CreateUnitTestObjectTypeIfDoesntExist();

            ResourceObject svStringAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeStringSV, null, false, AttributeType.String);
            ResourceObject mvStringAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeStringMV, null, true, AttributeType.String);
            ResourceObject svIntegerAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeIntegerSV, null, false, AttributeType.Integer);
            ResourceObject mvIntegerAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeIntegerMV, null, true, AttributeType.Integer);
            ResourceObject svReferenceAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeReferenceSV, null, false, AttributeType.Reference);
            ResourceObject mvReferenceAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeReferenceMV, null, true, AttributeType.Reference);
            ResourceObject svTextAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeTextSV, null, false, AttributeType.Text);
            ResourceObject mvTextAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeTextMV, null, true, AttributeType.Text);
            ResourceObject svDateTimeAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeDateTimeSV, null, false, AttributeType.DateTime);
            ResourceObject mvDateTimeAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeDateTimeMV, null, true, AttributeType.DateTime);
            ResourceObject svBinaryAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeBinarySV, null, false, AttributeType.Binary);
            ResourceObject mvBinaryAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeBinaryMV, null, true, AttributeType.Binary);
            ResourceObject svBooleanAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeBooleanSV, null, false, AttributeType.Boolean);
            ResourceObject accountNameAttribute = CreateAttributeTypeIfDoesntExist(AttributeNames.AccountName, null, false, AttributeType.String);

            CreateBindingIfDoesntExist(objectClass, svStringAttribute);
            CreateBindingIfDoesntExist(objectClass, mvStringAttribute);
            CreateBindingIfDoesntExist(objectClass, svIntegerAttribute);
            CreateBindingIfDoesntExist(objectClass, mvIntegerAttribute);
            CreateBindingIfDoesntExist(objectClass, svReferenceAttribute);
            CreateBindingIfDoesntExist(objectClass, mvReferenceAttribute);
            CreateBindingIfDoesntExist(objectClass, svTextAttribute);
            CreateBindingIfDoesntExist(objectClass, mvTextAttribute);
            CreateBindingIfDoesntExist(objectClass, svDateTimeAttribute);
            CreateBindingIfDoesntExist(objectClass, mvDateTimeAttribute);
            CreateBindingIfDoesntExist(objectClass, svBinaryAttribute);
            CreateBindingIfDoesntExist(objectClass, mvBinaryAttribute);
            CreateBindingIfDoesntExist(objectClass, svBooleanAttribute);
            CreateBindingIfDoesntExist(objectClass, accountNameAttribute);

            client.RefreshSchema();
        }

        private static ResourceObject CreateUnitTestObjectTypeIfDoesntExist()
        {
            ResourceObject testObject = client.GetResourceByKey(ObjectTypeNames.ObjectTypeDescription, AttributeNames.Name, Constants.UnitTestObjectTypeName);

            if (testObject != null)
            {
                return testObject;
            }

            testObject = client.CreateResource(ObjectTypeNames.ObjectTypeDescription);
            testObject.Attributes[AttributeNames.Name].SetValue(Constants.UnitTestObjectTypeName);
            testObject.Attributes[AttributeNames.DisplayName].SetValue(Constants.UnitTestObjectTypeName);

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
            testAttribute.Attributes[AttributeNames.DisplayName].SetValue(typeName);

            if (regex != null)
            {
                testAttribute.Attributes[AttributeNames.StringRegex].SetValue(regex);
            }

            testAttribute.Attributes[AttributeNames.Multivalued].SetValue(multivalued);
            testAttribute.Attributes[AttributeNames.DataType].SetValue(type.ToString());

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
            resource.Attributes[AttributeNames.BoundObjectType].SetValue(objectType.ObjectID);
            resource.Attributes[AttributeNames.BoundAttributeType].SetValue(attributeType.ObjectID);
            resource.Attributes[AttributeNames.Required].SetValue(false);

            client.SaveResource(resource);
        }
    }
}
