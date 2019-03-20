using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using System.Xml.Schema;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// A static class that reflects the schema of the Resource Management Service
    /// </summary>
    public static class ResourceManagementSchema
    {
        private static ManualResetEvent schemaLock;

        /// <summary>
        /// The internal dictionary containing the object type name to object type definition mapping
        /// </summary>
        private static ConcurrentDictionary<string, ObjectTypeDefinition> ObjectTypes { get; set; }

        /// <summary>
        /// Gets the list of attributes that are considered mandatory for all object classes
        /// </summary>
        internal static ReadOnlyCollection<string> MandatoryAttributes { get; private set; }

        /// <summary>
        /// Gets a list of attributes that are computed and cannot be changed
        /// </summary>
        public static ReadOnlyCollection<string> ComputedAttributes { get; private set; }

        /// <summary>
        /// A regular expression that validates an attribute name according to the resource management service's rules
        /// </summary>
        public static Regex AttributeNameValidationRegex { get; private set; }

        /// <summary>
        /// A regular expression that validates an attribute name according to the resource management service's rules
        /// </summary>
        public static Regex ObjectTypeNameValidationRegex { get; private set; }

        /// <summary>
        /// A value that indicates if the schema has been loaded from the Resource Management Service
        /// </summary>
        private static bool isLoaded;

        /// <summary>
        /// A list of elements contained in the XML schema from the Resource Management Service that should be ignored. Certain complex types are present
        /// in the XML definition that do not represent object classes
        /// </summary>
        private static List<string> ElementsToIgnore;

        /// <summary>
        /// Gets the location that currently loaded schema was obtained from
        /// </summary>
        public static Uri SchemaEndpoint { get; private set; }

        /// <summary>
        /// Initializes the static instance of the ResourceManagementSchema class
        /// </summary>
        static ResourceManagementSchema()
        {
            schemaLock = new ManualResetEvent(true);
            ResourceManagementSchema.ObjectTypes = new ConcurrentDictionary<string, ObjectTypeDefinition>();
            ResourceManagementSchema.ElementsToIgnore = new List<string>();
            ResourceManagementSchema.ElementsToIgnore.Add("ReferenceType");
            ResourceManagementSchema.ElementsToIgnore.Add("BinaryCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("DateTimeCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("IntegerCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("ReferenceCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("StringCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("TextCollectionType");

            ResourceManagementSchema.MandatoryAttributes = new ReadOnlyCollection<string>(new List<string>() { AttributeNames.ObjectType, AttributeNames.ObjectID });
            List<string> computedAttributes = new List<string>();
            computedAttributes.Add("Creator");
            computedAttributes.Add("CreatedTime");
            computedAttributes.Add("ExpectedRulesList");
            computedAttributes.Add("DetectedRulesList");
            computedAttributes.Add("DeletedTime");
            computedAttributes.Add("ResourceTime");
            computedAttributes.Add("ComputedMember");
            computedAttributes.Add("ComputedActor");
            ResourceManagementSchema.ComputedAttributes = new ReadOnlyCollection<string>(computedAttributes);
        }

        internal static void LoadSchema(EndpointManager e)
        {
            if (!isLoaded)
            {
                ResourceManagementSchema.RefreshSchema(e);
            }
        }

        /// <summary>
        /// Forces a download of the schema from the ResourceManagementService
        /// </summary>
        /// <remarks>
        /// This method should be called after making changes to ResourceObjects that form the schema
        /// </remarks>
        internal static void RefreshSchema(EndpointManager e)
        {
            try
            {
                ResourceManagementSchema.schemaLock.WaitOne();
                ResourceManagementSchema.schemaLock.Reset();
                MetadataSet set = ResourceManagementSchema.GetMetadataSet(e);
                ResourceManagementSchema.PopulateSchemaFromMetadata(set);
                ResourceManagementSchema.LoadNameValidationRegularExpressions();
                ResourceManagementSchema.SchemaEndpoint = e.MetadataEndpoint.Uri;
                isLoaded = true;
            }
            finally
            {
                ResourceManagementSchema.schemaLock.Set();
            }
        }

        /// <summary>
        /// Gets the data type of the specific attribute
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>An <c>AttributeType</c> value</returns>
        public static AttributeType GetAttributeType(string attributeName)
        {
            ResourceManagementSchema.schemaLock.WaitOne();

            ResourceManagementSchema.LoadSchema(ResourceManagementClient.EndpointManager);

            foreach (ObjectTypeDefinition objectType in ResourceManagementSchema.ObjectTypes.Values)
            {
                AttributeTypeDefinition attributeType = objectType.Attributes.FirstOrDefault(t => t.SystemName == attributeName);

                if (attributeType != null)
                {
                    return attributeType.Type;
                }
            }

            throw new NoSuchAttributeException(attributeName);
        }

        /// <summary>
        /// Gets an object type definition from the schema by name
        /// </summary>
        /// <param name="name">The system name of the object type</param>
        /// <returns>A ObjectTypeDefinition with a system name that matches the 'name' parameter</returns>
        /// <exception cref="NoSuchObjectTypeException">Throw when an object type could not be found in the schema with a matching name</exception>
        public static ObjectTypeDefinition GetObjectType(string name)
        {
            ResourceManagementSchema.schemaLock.WaitOne();

            ResourceManagementSchema.LoadSchema(ResourceManagementClient.EndpointManager);

            if (ResourceManagementSchema.ObjectTypes.ContainsKey(name))
            {
                return ResourceManagementSchema.ObjectTypes[name];
            }
            else
            {
                throw new NoSuchObjectTypeException(name);
            }
        }

        /// <summary>
        /// Gets a value indicating if the object type exists in the schema
        /// </summary>
        /// <param name="name">The system name of the object type</param>
        /// <returns>True if the object type exists in the schema, false if it does not</returns>
        public static bool ContainsObjectType(string name)
        {
            ResourceManagementSchema.schemaLock.WaitOne();

            ResourceManagementSchema.LoadSchema(ResourceManagementClient.EndpointManager);

            return ResourceManagementSchema.ObjectTypes.ContainsKey(name);
        }

        /// <summary>
        /// Gets each object type definition from the schema 
        /// </summary>
        /// <returns>An enumeration of ObjectTypeDefinitions</returns>
        public static IEnumerable<ObjectTypeDefinition> GetObjectTypes()
        {
            ResourceManagementSchema.schemaLock.WaitOne();

            ResourceManagementSchema.LoadSchema(ResourceManagementClient.EndpointManager);

            return ResourceManagementSchema.ObjectTypes.Values;
        }

        /// <summary>
        /// Gets a value indicating whether the specific attribute is multivalued
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>A value indicating whether the specific attribute is multivalued</returns>
        public static bool IsAttributeMultivalued(string attributeName)
        {
            ResourceManagementSchema.schemaLock.WaitOne();

            ResourceManagementSchema.LoadSchema(ResourceManagementClient.EndpointManager);

            foreach (ObjectTypeDefinition objectType in ResourceManagementSchema.ObjectTypes.Values)
            {
                AttributeTypeDefinition attributeType = objectType.Attributes.FirstOrDefault(t => t.SystemName == attributeName);

                if (attributeType != null)
                {
                    return attributeType.IsMultivalued;
                }
            }

            throw new NoSuchAttributeException(attributeName);
        }

        /// <summary>
        /// Gets the metadata set from the Resource Management Service
        /// </summary>
        /// <returns></returns>
        private static MetadataSet GetMetadataSet(EndpointManager e)
        {
            MetadataSet set;
            GetMetadata body = new GetMetadata();
            body.Dialect = Namespaces.XmlSchema;
            body.Identifier = string.Format("xs:schema/{0}", Namespaces.ResourceManagement);

            Message requestMessage = Message.CreateMessage(MessageVersion.Default, Namespaces.Get, new SerializerBodyWriter(body));

            Binding httpBinding = BindingManager.GetWsHttpBinding();

            ResourceManagementService.MetadataExchangeClient mex = new ResourceManagementService.MetadataExchangeClient(httpBinding, e.MetadataEndpoint);
            Message responseMessage = mex.Get(requestMessage);

            if (responseMessage.IsFault)
            {
                // We shouldn't get here as the WCF framework should convert the fault into an exception
                MessageFault fault = MessageFault.CreateFault(responseMessage, Int32.MaxValue);
                throw new FaultException(fault);
            }

            set = responseMessage.GetBody<MetadataSet>();
            return set;
        }

        /// <summary>
        /// Populates the internal definition of the schema based on the representation contained in the metadata set
        /// </summary>
        /// <param name="set">The metadata set describing the schema</param>
        private static void PopulateSchemaFromMetadata(MetadataSet set)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            if (set.MetadataSections.Count != 1)
            {
                throw new NotSupportedException("The metadata returned from the resource management service did not contain the expected number of sections");
            }

            MetadataSection section = set.MetadataSections[0];

            XmlSchema metadata = section.Metadata as XmlSchema;
            if (metadata == null)
            {
                throw new NotSupportedException("The metadata returned from the resource management service did not contain the expected metadata section");
            }

            ResourceManagementSchema.ObjectTypes.Clear();
            
            foreach (XmlSchemaComplexType complexType in metadata.Items.OfType<XmlSchemaComplexType>())
            {
                if (!ResourceManagementSchema.ElementsToIgnore.Contains(complexType.Name))
                {
                    ObjectTypeDefinition definition = new ObjectTypeDefinition(complexType);
                    ResourceManagementSchema.ObjectTypes.TryAdd(definition.SystemName, definition);
                }
            }
        }

        /// <summary>
        /// Validates that an attribute name contains only valid characters
        /// </summary>
        /// <param name="attributeName">The name of the attribute to validate</param>
        public static void ValidateAttributeName(string attributeName)
        {
            if (!ResourceManagementSchema.AttributeNameValidationRegex.IsMatch(attributeName))
            {
                throw new ArgumentException("The attribute name contains invalid characters", nameof(attributeName));
            }
        }

        /// <summary>
        /// Validates that an object type name contains only valid characters
        /// </summary>
        /// <param name="objectTypeName">The name of the object type to validate</param>
        public static void ValidateObjectTypeName(string objectTypeName)
        {
            if (!ResourceManagementSchema.ObjectTypeNameValidationRegex.IsMatch(objectTypeName))
            {
                throw new ArgumentException("The object type name contains invalid characters", nameof(objectTypeName));
            }
        }

        private static void LoadNameValidationRegularExpressions()
        {
            ObjectTypeDefinition objectTypeDefinition = ResourceManagementSchema.ObjectTypes[ObjectTypeNames.AttributeTypeDescription];
            AttributeTypeDefinition nameAttribute = objectTypeDefinition.Attributes.First(t => t.SystemName == AttributeNames.Name);
            ResourceManagementSchema.AttributeNameValidationRegex = new Regex(nameAttribute.Regex);

            objectTypeDefinition = ResourceManagementSchema.ObjectTypes[ObjectTypeNames.ObjectTypeDescription];
            nameAttribute = objectTypeDefinition.Attributes.First(t => t.SystemName == AttributeNames.Name);
            ResourceManagementSchema.ObjectTypeNameValidationRegex = new Regex(nameAttribute.Regex);
        }
    }
}