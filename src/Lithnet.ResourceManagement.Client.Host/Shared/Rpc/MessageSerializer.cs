using System;
using System.ServiceModel.Channels;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using Newtonsoft.Json;

namespace Lithnet.ResourceManagement.Client.Host
{
    public class MessageSerializer : JsonConverter, IMessagePackFormatter<Message>, IFormatterResolver
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Message).IsAssignableFrom(objectType);
        }

        public Message Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var message = reader.ReadString();
            return MessageService.Deserialize(message);
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (typeof(T) == typeof(Message))
            {
                return (IMessagePackFormatter<T>)this;
            }

            return null;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var messageS = reader.Value as string;
            return MessageService.Deserialize(messageS);
        }

        public void Serialize(ref MessagePackWriter writer, Message value, MessagePackSerializerOptions options)
        {
            var result = MessageService.Serialize(value);
            var bytes = UTF8Encoding.UTF8.GetBytes(result);
            writer.WriteString(bytes);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var message = value as Message;
            var result = MessageService.Serialize(message);
            serializer.Serialize(writer, result);
        }
    }
}