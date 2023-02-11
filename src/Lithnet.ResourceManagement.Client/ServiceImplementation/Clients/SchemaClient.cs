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
using Nito.AsyncEx;

namespace Lithnet.ResourceManagement.Client
{
    internal class SchemaClient : ISchemaClient
    {
        private readonly IClient client;
        private readonly AsyncReaderWriterLock readWriteLock = new AsyncReaderWriterLock();

        /// <summary>
        /// The internal dictionary containing the object type name to object type definition mapping
        /// </summary>
        private ConcurrentDictionary<string, ObjectTypeDefinition> ObjectTypes { get; }

        private ConcurrentDictionary<string, AttributeTypeDefinition> AttributeTypes { get; }

        private ConcurrentDictionary<string, AttributeTypeDefinition> AttributeTypesCaseInsenstive { get; } = new ConcurrentDictionary<string, AttributeTypeDefinition>(StringComparer.OrdinalIgnoreCase);

        private ConcurrentDictionary<string, ObjectTypeDefinition> ObjectTypesCaseInsenstive { get; } = new ConcurrentDictionary<string, ObjectTypeDefinition>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// A regular expression that validates an attribute name according to the resource management service's rules
        /// </summary>
        public Regex AttributeNameValidationRegex { get; private set; }

        /// <summary>
        /// A regular expression that validates an attribute name according to the resource management service's rules
        /// </summary>
        public Regex ObjectTypeNameValidationRegex { get; private set; }

        /// <summary>
        /// A value that indicates if the schema has been loaded from the Resource Management Service
        /// </summary>
        private bool isLoaded;

        private SchemaClient()
        {
            this.ObjectTypes = new ConcurrentDictionary<string, ObjectTypeDefinition>();
            this.AttributeTypes = new ConcurrentDictionary<string, AttributeTypeDefinition>();
        }

        public SchemaClient(IClient client) : this()
        {
            this.client = client;
        }

        public async Task LoadSchemaAsync()
        {
            await this.EnsureSchemaLoadedAsync();
        }

        private async Task EnsureSchemaLoadedAsync()
        {
            if (this.isLoaded)
            {
                return;
            }

            await this.RefreshSchemaAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Forces a download of the schema from the ResourceManagementService
        /// </summary>
        /// <remarks>
        /// This method should be called after making changes to ResourceObjects that form the schema
        /// </remarks>
        public async Task RefreshSchemaAsync()
        {
            using (await this.readWriteLock.WriterLockAsync())
            {
                MetadataSet set = await this.GetMetadataSetAsync().ConfigureAwait(false);
                this.PopulateSchemaFromMetadata(set);
                this.LoadNameValidationRegularExpressions();
                this.isLoaded = true;
            }
        }

        /// <summary>
        /// Gets the data type of the specific attribute
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>An <c>AttributeType</c> value</returns>
        public async Task<AttributeType> GetAttributeTypeAsync(string attributeName)
        {
            await this.EnsureSchemaLoadedAsync().ConfigureAwait(false);

            using (await this.readWriteLock.ReaderLockAsync())
            {
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
        }

        public async Task<AttributeTypeDefinition> GetAttributeAsync(string attributeName)
        {
            await this.EnsureSchemaLoadedAsync().ConfigureAwait(false);

            using (await this.readWriteLock.ReaderLockAsync())
            {
                if (this.AttributeTypes.TryGetValue(attributeName, out var definition))
                {
                    return definition;
                }

                if (this.AttributeTypesCaseInsenstive.TryGetValue(attributeName, out definition))
                {
                    return definition;
                }

                throw new NoSuchAttributeException(attributeName);
            }
        }

        /// <summary>
        /// Gets an object type definition from the schema by name
        /// </summary>
        /// <param name="name">The system name of the object type</param>
        /// <returns>A ObjectTypeDefinition with a system name that matches the 'name' parameter</returns>
        /// <exception cref="NoSuchObjectTypeException">Throw when an object type could not be found in the schema with a matching name</exception>
        public async Task<ObjectTypeDefinition> GetObjectTypeAsync(string name)
        {
            await this.EnsureSchemaLoadedAsync().ConfigureAwait(false);

            using (await this.readWriteLock.ReaderLockAsync())
            {
                if (this.ObjectTypes.TryGetValue(name, out var definition))
                {
                    return definition;
                }

                if (this.ObjectTypesCaseInsenstive.TryGetValue(name, out definition))
                {
                    return definition;
                }

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
            await this.EnsureSchemaLoadedAsync().ConfigureAwait(false);

            using (await this.readWriteLock.ReaderLockAsync())
            {
                return this.ObjectTypes.ContainsKey(name);
            }
        }

        /// <summary>
        /// Gets each object type definition from the schema 
        /// </summary>
        /// <returns>An enumeration of ObjectTypeDefinitions</returns>
        public async IAsyncEnumerable<ObjectTypeDefinition> GetObjectTypesAsync()
        {
            await this.EnsureSchemaLoadedAsync().ConfigureAwait(false);

            using (await this.readWriteLock.ReaderLockAsync())
            {
                foreach (var item in this.ObjectTypes.Values)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specific attribute is multivalued
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>A value indicating whether the specific attribute is multivalued</returns>
        public async Task<bool> IsAttributeMultivaluedAsync(string attributeName)
        {
            var attribute = await this.GetAttributeAsync(attributeName).ConfigureAwait(false);
            return attribute.IsMultivalued;
        }


        public async Task<AttributeTypeDefinition> GetAttributeDefinitionAsync(string attributeName)
        {
            return await this.GetAttributeAsync(attributeName).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that an attribute name contains only valid characters
        /// </summary>
        /// <param name="attributeName">The name of the attribute to validate</param>
        public async Task ValidateAttributeNameAsync(string attributeName)
        {
            await this.EnsureSchemaLoadedAsync().ConfigureAwait(false);

            using (await this.readWriteLock.ReaderLockAsync())
            {
                if (!this.AttributeNameValidationRegex.IsMatch(attributeName))
                {
                    throw new ArgumentException("The attribute name contains invalid characters", nameof(attributeName));
                }
            }
        }

        /// <summary>
        /// Validates that an object type name contains only valid characters
        /// </summary>
        /// <param name="objectTypeName">The name of the object type to validate</param>
        public async Task ValidateObjectTypeNameAsync(string objectTypeName)
        {
            await this.EnsureSchemaLoadedAsync().ConfigureAwait(false);

            using (await this.readWriteLock.ReaderLockAsync())
            {
                if (!this.ObjectTypeNameValidationRegex.IsMatch(objectTypeName))
                {
                    throw new ArgumentException("The object type name contains invalid characters", nameof(objectTypeName));
                }
            }
        }

        public async Task<string> GetCorrectObjectTypeNameCaseAsync(string name)
        {
            var definition = await this.GetObjectTypeAsync(name).ConfigureAwait(false);
            return definition.SystemName;
        }

        public async Task<string> GetCorrectAttributeNameCaseAsync(string name)
        {
            var definition = await this.GetAttributeAsync(name).ConfigureAwait(false);
            return definition.SystemName;
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

            var channel = await this.client.GetSchemaChannelAsync();

            using (Message responseMessage = await channel.GetAsync(requestMessage).ConfigureAwait(false))
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
            this.ObjectTypesCaseInsenstive.Clear();
            this.AttributeTypes.Clear();
            this.AttributeTypesCaseInsenstive.Clear();

            foreach (XmlSchemaComplexType complexType in metadata.Items.OfType<XmlSchemaComplexType>())
            {
                if (!SchemaConstants.ElementsToIgnore.Contains(complexType.Name))
                {
                    ObjectTypeDefinition definition = new ObjectTypeDefinition(complexType);
                    this.ObjectTypes.TryAdd(definition.SystemName, definition);

                    if (this.ObjectTypesCaseInsenstive.ContainsKey(definition.SystemName))
                    {
                        this.ObjectTypesCaseInsenstive.TryRemove(definition.SystemName, out _);
                    }
                    else
                    {
                        this.ObjectTypesCaseInsenstive.TryAdd(definition.SystemName, definition);
                    }

                    foreach (var attribute in definition.Attributes)
                    {
                        if (!this.AttributeTypes.ContainsKey(attribute.SystemName))
                        {
                            this.AttributeTypes.TryAdd(attribute.SystemName, attribute);

                            if (this.AttributeTypesCaseInsenstive.ContainsKey(attribute.SystemName))
                            {
                                this.AttributeTypesCaseInsenstive.TryRemove(attribute.SystemName, out _);
                            }
                            else
                            {
                                this.AttributeTypesCaseInsenstive.TryAdd(attribute.SystemName, attribute);
                            }
                        }
                    }
                }
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