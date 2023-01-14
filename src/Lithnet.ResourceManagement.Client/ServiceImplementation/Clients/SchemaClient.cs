using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client
{
    internal class SchemaClient : ISchemaClient
    {
        private ManualResetEvent schemaLock;
        private IMetadataExchange channel;

        /// <summary>
        /// The internal dictionary containing the object type name to object type definition mapping
        /// </summary>
        private ConcurrentDictionary<string, ObjectTypeDefinition> ObjectTypes
        {
            get; set;
        }

        /// <summary>
        /// A regular expression that validates an attribute name according to the resource management service's rules
        /// </summary>
        public Regex AttributeNameValidationRegex
        {
            get; private set;
        }

        /// <summary>
        /// A regular expression that validates an attribute name according to the resource management service's rules
        /// </summary>
        public Regex ObjectTypeNameValidationRegex
        {
            get; private set;
        }

        /// <summary>
        /// A value that indicates if the schema has been loaded from the Resource Management Service
        /// </summary>
        private bool isLoaded;

        private SchemaClient()
        {
            this.schemaLock = new ManualResetEvent(true);
            this.ObjectTypes = new ConcurrentDictionary<string, ObjectTypeDefinition>();
        }

        public SchemaClient(IMetadataExchange channel) : this()
        {
            this.channel = channel;
        }

        public async Task LoadSchemaAsync()
        {
            if (!this.isLoaded)
            {
                await this.RefreshSchemaAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Forces a download of the schema from the ResourceManagementService
        /// </summary>
        /// <remarks>
        /// This method should be called after making changes to ResourceObjects that form the schema
        /// </remarks>
        public async Task RefreshSchemaAsync()
        {
            try
            {
                this.schemaLock.WaitOne();
                this.schemaLock.Reset();
                MetadataSet set = await this.GetMetadataSetAsync().ConfigureAwait(false);
                this.PopulateSchemaFromMetadata(set);
                this.LoadNameValidationRegularExpressions();
                this.isLoaded = true;
            }
            finally
            {
                this.schemaLock.Set();
            }
        }

        /// <summary>
        /// Gets the data type of the specific attribute
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>An <c>AttributeType</c> value</returns>
        public async Task<AttributeType> GetAttributeTypeAsync(string attributeName)
        {
            this.schemaLock.WaitOne();

            await this.LoadSchemaAsync().ConfigureAwait(false);

            foreach (ObjectTypeDefinition objectType in this.ObjectTypes.Values)
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
        public async Task<ObjectTypeDefinition> GetObjectTypeAsync(string name)
        {
            this.schemaLock.WaitOne();

            await this.LoadSchemaAsync().ConfigureAwait(false);

            if (this.ObjectTypes.ContainsKey(name))
            {
                return this.ObjectTypes[name];
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
        public async Task<bool> ContainsObjectTypeAsync(string name)
        {
            this.schemaLock.WaitOne();

            await this.LoadSchemaAsync().ConfigureAwait(false);

            return this.ObjectTypes.ContainsKey(name);
        }

        /// <summary>
        /// Gets each object type definition from the schema 
        /// </summary>
        /// <returns>An enumeration of ObjectTypeDefinitions</returns>
        public async IAsyncEnumerable<ObjectTypeDefinition> GetObjectTypesAsync()
        {
            this.schemaLock.WaitOne();

            await this.LoadSchemaAsync().ConfigureAwait(false);

            foreach (var item in this.ObjectTypes.Values)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specific attribute is multivalued
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>A value indicating whether the specific attribute is multivalued</returns>
        public async Task<bool> IsAttributeMultivaluedAsync(string attributeName)
        {
            this.schemaLock.WaitOne();

            await this.LoadSchemaAsync().ConfigureAwait(false);

            foreach (ObjectTypeDefinition objectType in this.ObjectTypes.Values)
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
        private async Task<MetadataSet> GetMetadataSetAsync()
        {
            MetadataSet set;
            GetMetadata body = new GetMetadata();
            body.Dialect = Namespaces.XmlSchema;
            body.Identifier = string.Format("xs:schema/{0}", Namespaces.ResourceManagement);

            Message requestMessage = Message.CreateMessage(MessageVersion.Default, Namespaces.Get, new SerializerBodyWriter(body));

            using (Message responseMessage = await this.channel.GetAsync(requestMessage).ConfigureAwait(false))
            {
                if (responseMessage.IsFault)
                {
                    // We shouldn't get here as the WCF framework should convert the fault into an exception
                    MessageFault fault = MessageFault.CreateFault(responseMessage, Int32.MaxValue);
                    throw new FaultException(fault, "create");
                }

                set = responseMessage.GetBody<MetadataSet>();
                return set;
            }
        }

        /// <summary>
        /// Populates the internal definition of the schema based on the representation contained in the metadata set
        /// </summary>
        /// <param name="set">The metadata set describing the schema</param>
        private void PopulateSchemaFromMetadata(MetadataSet set)
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

            this.ObjectTypes.Clear();

            foreach (XmlSchemaComplexType complexType in metadata.Items.OfType<XmlSchemaComplexType>())
            {
                if (!SchemaConstants.ElementsToIgnore.Contains(complexType.Name))
                {
                    ObjectTypeDefinition definition = new ObjectTypeDefinition(complexType);
                    this.ObjectTypes.TryAdd(definition.SystemName, definition);
                }
            }
        }

        /// <summary>
        /// Validates that an attribute name contains only valid characters
        /// </summary>
        /// <param name="attributeName">The name of the attribute to validate</param>
        public void ValidateAttributeName(string attributeName)
        {
            if (!this.AttributeNameValidationRegex.IsMatch(attributeName))
            {
                throw new ArgumentException("The attribute name contains invalid characters", nameof(attributeName));
            }
        }

        /// <summary>
        /// Validates that an object type name contains only valid characters
        /// </summary>
        /// <param name="objectTypeName">The name of the object type to validate</param>
        public void ValidateObjectTypeName(string objectTypeName)
        {
            if (!this.ObjectTypeNameValidationRegex.IsMatch(objectTypeName))
            {
                throw new ArgumentException("The object type name contains invalid characters", nameof(objectTypeName));
            }
        }

        private void LoadNameValidationRegularExpressions()
        {
            ObjectTypeDefinition objectTypeDefinition = this.ObjectTypes[ObjectTypeNames.AttributeTypeDescription];
            AttributeTypeDefinition nameAttribute = objectTypeDefinition.Attributes.First(t => t.SystemName == AttributeNames.Name);
            this.AttributeNameValidationRegex = new Regex(nameAttribute.Regex);

            objectTypeDefinition = this.ObjectTypes[ObjectTypeNames.ObjectTypeDescription];
            nameAttribute = objectTypeDefinition.Attributes.First(t => t.SystemName == AttributeNames.Name);
            this.ObjectTypeNameValidationRegex = new Regex(nameAttribute.Regex);
        }
    }
}