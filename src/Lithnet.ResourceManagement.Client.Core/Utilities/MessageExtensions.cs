using System;
using System.ServiceModel.Channels;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    internal static class MessageExtensions
    {
        public static void AddHeader(this Message message, string name, object value)
        {
            message.AddHeader(Namespaces.ResourceManagement, name, value, false);
        }

        public static void AddHeader(this Message message, string xsdName, string name, object value, bool mustUnderstand)
        {
            if (message.Headers.FindHeader(name, xsdName) <= 0)
            {
                message.Headers.Add(MessageHeader.CreateHeader(name, xsdName, value?.ToString(), mustUnderstand));
            }
        }

        public static void AddHeader(this Message message, string xsdName, string name, object value)
        {
            if (message.Headers.FindHeader(name, xsdName) <= 0)
            {
                message.Headers.Add(MessageHeader.CreateHeader(name, xsdName, value?.ToString()));
            }
        }

        public static T DeserializeMessageWithPayload<T>(this Message messageWithPayload)
        {
            if (messageWithPayload.IsEmpty)
            {
                return default(T);
            }

            if (typeof(IXmlSerializable).IsAssignableFrom(typeof(T)))
            {
                IXmlSerializable serializable = (IXmlSerializable)Activator.CreateInstance(typeof(T));
                serializable.ReadXml(messageWithPayload.GetReaderAtBodyContents());
                return (T)serializable;
            }

            XmlSerializer xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(messageWithPayload.GetReaderAtBodyContents());
        }

        public static T DeserializeMessageWithPayload<T>(this MessageFault messageWithPayload)
        {
            if (!messageWithPayload.HasDetail)
            {
                return default(T);
            }

            if (typeof(IXmlSerializable).IsAssignableFrom(typeof(T)))
            {
                IXmlSerializable serializable = (IXmlSerializable)Activator.CreateInstance(typeof(T));
                serializable.ReadXml(messageWithPayload.GetReaderAtDetailContents());
                return (T)serializable;
            }

            XmlSerializer xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(messageWithPayload.GetReaderAtDetailContents());
        }

        public static void ThrowOnFault(this Message message)
        {
            if (message.IsFault)
            {
                MessageFault fault = MessageFault.CreateFault(message, Int32.MaxValue);
                throw ServiceFaultTranslator.GetExceptionFromFaultMessage(fault);
            }
        }
    }
}
