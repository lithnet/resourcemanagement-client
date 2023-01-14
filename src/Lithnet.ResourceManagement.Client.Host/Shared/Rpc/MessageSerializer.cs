using System;
using System.ServiceModel.Channels;
using Newtonsoft.Json;

namespace Lithnet.ResourceManagement.Client.Host
{
    public class MessageSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Message).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var messageS = reader.Value as string;
            return MessageService.Deserialize(messageS);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var message = value as Message;
            var result = MessageService.Serialize(message);
            serializer.Serialize(writer, result);
        }
    }
}