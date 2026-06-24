using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [SetUpFixture]
    public static class Setup
    {
        public delegate ResourceManagementClient ResourceManagementClientMapper(ConnectionMode mode);

        static ResourceManagementClient client;

        [OneTimeSetUp]
        public static void InitializeTestEngine()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
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

            builder.ConfigureServices((context, services) =>
            {
                services.AddOptions<ResourceManagementClientOptions>();

                services.Configure<ResourceManagementClientOptions>(context.Configuration.GetSection("LithnetResourceManagementClient"));
                services.AddSingleton<ResourceManagementClient>();

                services.AddSingleton<ResourceManagementClientMapper>(provider => (mode) =>
                {
                    var op = provider.GetRequiredService<IOptions<ResourceManagementClientOptions>>();
                    var n = ResourceManagementClientOptions.Clone(op.Value);
                    n.ConnectionMode = mode;

                    return new ResourceManagementClient(n);
                });
                
#if NETFRAMEWORK
                services.PostConfigure<ResourceManagementClientOptions>((op) =>
                {
                    if (!FrameworkUtilities.IsFramework)
                    {
                        return;
                    }

                    //var op2 = ClientConfigurationSection.GetOptionsFromConfiguration();
                    //if (op2 != null)
                    //{
                    //    op.BaseUri = op2.BaseUri;
                    //    op.Spn = op2.Spn;
                    //    op.ConcurrentConnectionLimit = op2.ConcurrentConnectionLimit;
                    //    op.ConnectTimeoutSeconds = op2.ConnectTimeoutSeconds;
                    //    op.SendTimeoutSeconds = op2.SendTimeoutSeconds;
                    //    op.RecieveTimeoutSeconds = op2.RecieveTimeoutSeconds;
                    //    op.Password = op2.Password;
                    //    op.Username = op2.Username;
                    //}
                });
#endif
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

        private static IResourceObject CreateReferenceTestObjectIfDoesntExist(string accountName)
        {
            IResourceObject testObject = client.GetResourceByKey(Constants.UnitTestObjectTypeName, AttributeNames.AccountName, accountName);

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
            IResourceObject objectClass = CreateUnitTestObjectTypeIfDoesntExist();

            IResourceObject svStringAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeStringSV, null, false, AttributeType.String);
            Constants.AttributeStringSVDef = client.GetAttributeDefinition(Constants.AttributeStringSV);

            IResourceObject mvStringAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeStringMV, null, true, AttributeType.String);
            Constants.AttributeStringMVDef = client.GetAttributeDefinition(Constants.AttributeStringMV);

            IResourceObject svIntegerAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeIntegerSV, null, false, AttributeType.Integer);
            Constants.AttributeIntegerSVDef = client.GetAttributeDefinition(Constants.AttributeIntegerSV);

            IResourceObject mvIntegerAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeIntegerMV, null, true, AttributeType.Integer);
            Constants.AttributeIntegerMVDef = client.GetAttributeDefinition(Constants.AttributeIntegerMV);

            IResourceObject svReferenceAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeReferenceSV, null, false, AttributeType.Reference);
            Constants.AttributeReferenceSVDef = client.GetAttributeDefinition(Constants.AttributeReferenceSV);

            IResourceObject mvReferenceAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeReferenceMV, null, true, AttributeType.Reference);
            Constants.AttributeReferenceMVDef = client.GetAttributeDefinition(Constants.AttributeReferenceMV);

            IResourceObject svTextAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeTextSV, null, false, AttributeType.Text);
            Constants.AttributeTextSVDef = client.GetAttributeDefinition(Constants.AttributeTextSV);

            IResourceObject mvTextAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeTextMV, null, true, AttributeType.Text);
            Constants.AttributeTextMVDef = client.GetAttributeDefinition(Constants.AttributeTextMV);

            IResourceObject svDateTimeAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeDateTimeSV, null, false, AttributeType.DateTime);
            Constants.AttributeDateTimeSVDef = client.GetAttributeDefinition(Constants.AttributeDateTimeSV);

            IResourceObject mvDateTimeAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeDateTimeMV, null, true, AttributeType.DateTime);
            Constants.AttributeDateTimeMVDef = client.GetAttributeDefinition(Constants.AttributeDateTimeMV);

            IResourceObject svBinaryAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeBinarySV, null, false, AttributeType.Binary);
            Constants.AttributeBinarySVDef = client.GetAttributeDefinition(Constants.AttributeBinarySV);

            IResourceObject mvBinaryAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeBinaryMV, null, true, AttributeType.Binary);
            Constants.AttributeBinaryMVDef = client.GetAttributeDefinition(Constants.AttributeBinaryMV);

            IResourceObject svBooleanAttribute = CreateAttributeTypeIfDoesntExist(Constants.AttributeBooleanSV, null, false, AttributeType.Boolean);
            Constants.AttributeBooleanSVDef = client.GetAttributeDefinition(Constants.AttributeBooleanSV);

            IResourceObject accountNameAttribute = CreateAttributeTypeIfDoesntExist(AttributeNames.AccountName, null, false, AttributeType.String);

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

        private static IResourceObject CreateUnitTestObjectTypeIfDoesntExist()
        {
            IResourceObject testObject = client.GetResourceByKey(ObjectTypeNames.ObjectTypeDescription, AttributeNames.Name, Constants.UnitTestObjectTypeName);

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

        private static IResourceObject CreateAttributeTypeIfDoesntExist(string typeName, string regex, bool multivalued, AttributeType type)
        {
            IResourceObject testAttribute = client.GetResourceByKey(ObjectTypeNames.AttributeTypeDescription, AttributeNames.Name, typeName);

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

        private static void CreateBindingIfDoesntExist(IResourceObject objectType, IResourceObject attributeType)
        {
            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add(AttributeNames.BoundObjectType, objectType.ObjectID.Value);
            keys.Add(AttributeNames.BoundAttributeType, attributeType.ObjectID.Value);

            IResourceObject testObject = client.GetResourceByKey(ObjectTypeNames.BindingDescription, keys);

            if (testObject != null)
            {
                return;
            }

            IResourceObject resource = client.CreateResource(ObjectTypeNames.BindingDescription);
            resource.Attributes[AttributeNames.BoundObjectType].SetValue(objectType.ObjectID);
            resource.Attributes[AttributeNames.BoundAttributeType].SetValue(attributeType.ObjectID);
            resource.Attributes[AttributeNames.Required].SetValue(false);

            client.SaveResource(resource);
        }
    }
}
