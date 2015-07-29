using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.WSMetadataExchange;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using System.Xml.Schema;
using System.ServiceModel;
using System.Collections.ObjectModel;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// A static class that reflects the schema of the Resource Management Service
    /// </summary>
    public static class ResourceManagementSchema
    {
        /// <summary>
        /// The internal dictionary containing the object type name to object type definition mapping
        /// </summary>
        public static Dictionary<string, ObjectTypeDefinition> ObjectTypes;

        /// <summary>
        /// Gets the list of attributes that are considered mandatory for all object classes
        /// </summary>
        internal static ReadOnlyCollection<string> MandatoryAttributes { get; private set; }

        /// <summary>
        /// The object used to synchronize access updates to the schema from multiple threads
        /// </summary>
        private static object lockObject;

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
        /// Initializes the static instance of the ResourceManagementSchema class
        /// </summary>
        static ResourceManagementSchema()
        {
            lockObject = new object();
            ResourceManagementSchema.ObjectTypes = new Dictionary<string, ObjectTypeDefinition>();
            ResourceManagementSchema.ElementsToIgnore = new List<string>();
            ResourceManagementSchema.ElementsToIgnore.Add("ReferenceType");
            ResourceManagementSchema.ElementsToIgnore.Add("BinaryCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("DateTimeCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("IntegerCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("ReferenceCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("StringCollectionType");
            ResourceManagementSchema.ElementsToIgnore.Add("TextCollectionType");

            ResourceManagementSchema.MandatoryAttributes = new ReadOnlyCollection<string>(new List<string>() { AttributeNames.ObjectType, AttributeNames.ObjectID });
        }

        /// <summary>
        /// Loads the schema from the Resource Management Service if it has not already been loaded. If the schema has been loaded, no actions are performed
        /// </summary>
        public static void LoadSchema()
        {
            lock (lockObject)
            {
                if (!isLoaded)
                {
                    ResourceManagementSchema.RefreshSchema();
                }
            }
        }

        /// <summary>
        /// Forces a download of the schema from the ResourceManagementService
        /// </summary>
        /// <remarks>
        /// This method should be called after making changes to ResourceObjects that form the schema
        /// </remarks>
        public static void RefreshSchema()
        {
            lock (lockObject)
            {
                ResourceManagementSchema.ObjectTypes = new Dictionary<string, ObjectTypeDefinition>();

                MetadataSet set = ResourceManagementSchema.GetMetadataSet();
                ResourceManagementSchema.PopulateSchemaFromMetadata(set);
                isLoaded = true;
            }
        }

        public static AttributeType GetAttributeType(string attributeName)
        {
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

        public static bool IsAttributeMultivalued(string attributeName)
        {
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
        private static MetadataSet GetMetadataSet()
        {
            MetadataSet set;
            GetMetadata body = new GetMetadata();
            body.Dialect = Namespaces.XmlSchema;
            body.Identifier = string.Format("xs:schema/{0}", Namespaces.ResourceManagement);

            Message requestMessage = Message.CreateMessage(MessageVersion.Default, Namespaces.Get, new SerializerBodyWriter(body));

            Binding httpBinding = BindingManager.GetWsHttpBinding();

            ResourceManagementService.MetadataExchangeClient mex = new ResourceManagementService.MetadataExchangeClient(httpBinding, ResourceManagementClient.EndpointManager.MetadataEndpoint);
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
        /// Populates the internal defintion of the schema based on the representation contained in the metadata set
        /// </summary>
        /// <param name="set">The metadata set describing the schema</param>
        private static void PopulateSchemaFromMetadata(MetadataSet set)
        {
            if (set == null)
            {
                throw new ArgumentNullException("set");
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

            foreach (XmlSchemaComplexType complexType in metadata.Items.OfType<XmlSchemaComplexType>())
            {
                if (!ResourceManagementSchema.ElementsToIgnore.Contains(complexType.Name))
                {
                    ObjectTypeDefinition definition = new ObjectTypeDefinition(complexType);
                    ResourceManagementSchema.ObjectTypes.Add(definition.SystemName, definition);
                }
            }
        }
    }
}