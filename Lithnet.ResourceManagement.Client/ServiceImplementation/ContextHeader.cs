
using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Lithnet.ResourceManagement.Client
{
    internal class ContextHeader : MessageHeader
    {
        public string Value { get; }

        public ContextHeader(string contextValue)
        {
            this.Value = contextValue;
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.WriteStartElement("Property", this.Namespace);
            writer.WriteAttributeString("name", null, "instanceId");
            writer.WriteValue(this.Value);
            writer.WriteEndElement();
        }

        // Properties
        public override string Name => "Context";

        /// <summary>Gets the namespace of the message header.</summary>
        /// <returns>The namespace of the message header.</returns>
        public override string Namespace => Namespaces.Context;
    }
}