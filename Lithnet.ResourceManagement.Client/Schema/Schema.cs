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

namespace Lithnet.ResourceManagement.Client
{
    public static class Schema
    {
        public static Dictionary<string, ObjectTypeDefinition> ObjectTypes;

        private static object lockObject;

        private static bool isLoaded;

        private static List<string> ElementsToIgnore;

        static Schema()
        {
            lockObject = new object();
            Schema.ObjectTypes = new Dictionary<string, ObjectTypeDefinition>();
            Schema.ElementsToIgnore = new List<string>();
            Schema.ElementsToIgnore.Add("ReferenceType");
            Schema.ElementsToIgnore.Add("BinaryCollectionType");
            Schema.ElementsToIgnore.Add("DateTimeCollectionType");
            Schema.ElementsToIgnore.Add("IntegerCollectionType");
            Schema.ElementsToIgnore.Add("ReferenceCollectionType");
            Schema.ElementsToIgnore.Add("StringCollectionType");
            Schema.ElementsToIgnore.Add("TextCollectionType");
        }

        public static void LoadSchema()
        {
            lock (lockObject)
            {
                if (!isLoaded)
                {
                    Schema.RefreshSchema();
                }
            }
        }

        public static void RefreshSchema()
        {
            lock (lockObject)
            {
                Schema.ObjectTypes = new Dictionary<string, ObjectTypeDefinition>();

                MetadataSet set = Schema.GetMetadataSet();
                Schema.PopulateSchemaFromMetadata(set);
                isLoaded = true;
            }
        }

        private static MetadataSet GetMetadataSet()
        {
            MetadataSet set;
            GetMetadata body = new GetMetadata();
            body.Dialect = "http://www.w3.org/2001/XMLSchema";
            body.Identifier = string.Format("xs:schema/{0}", "http://schemas.microsoft.com/2006/11/ResourceManagement");

            Message requestMessage = Message.CreateMessage(MessageVersion.Default, "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get", new SerializerBodyWriter(body));

            Lithnet.ResourceManagement.Client.ResourceManagementService.MetadataExchangeClient mex = new ResourceManagementService.MetadataExchangeClient();
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

        private static void PopulateSchemaFromMetadata(MetadataSet set)
        {
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
                if (complexType == null)
                {
                    continue;
                }
                if (!Schema.ElementsToIgnore.Contains(complexType.Name))
                {
                    ObjectTypeDefinition definition = new ObjectTypeDefinition(complexType);
                    Schema.ObjectTypes.Add(definition.SystemName, definition);
                }
            }
        }
    }
}