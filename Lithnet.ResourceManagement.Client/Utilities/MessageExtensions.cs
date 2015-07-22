using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.ResourceManagement.WebServices.Exceptions;
using Microsoft.ResourceManagement.WebServices.Client;
using Microsoft.ResourceManagement.WebServices.WSTransfer;
using Microsoft.ResourceManagement.WebServices;

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
                message.Headers.Add(MessageHeader.CreateHeader(name, xsdName, (value == null) ? null : value.ToString(), mustUnderstand));
            }
        }

        public static void AddHeader(this Message message, string xsdName, string name, object value)
        {
            if (message.Headers.FindHeader(name, xsdName) <= 0)
            {
                message.Headers.Add(MessageHeader.CreateHeader(name, xsdName, (value == null) ? null : value.ToString()));
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
