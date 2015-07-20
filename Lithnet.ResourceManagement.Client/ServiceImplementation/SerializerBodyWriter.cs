using System;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    [DebuggerStepThrough]
    internal class SerializerBodyWriter : BodyWriter
    {
        private readonly object toSerialize;

        public SerializerBodyWriter(object toSerialize)
            : base(false)
        {
            this.toSerialize = toSerialize;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            if (toSerialize != null)
            {
                IXmlSerializable serializable = toSerialize as IXmlSerializable;
                if (serializable != null)
                {
                    serializable.WriteXml(writer);
                }
                else
                {
                    XmlSerializer xs = new XmlSerializer(toSerialize.GetType());
                    xs.Serialize(writer, toSerialize);
                }
            }
        }
    }       
}
