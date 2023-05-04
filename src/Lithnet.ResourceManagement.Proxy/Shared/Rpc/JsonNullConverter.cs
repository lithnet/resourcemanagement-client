using System;
using Newtonsoft.Json;

namespace Lithnet.ResourceManagement.Client
{
    internal class JsonNullConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == NullFormatter.FaultCodeDataType)
            {
                return true;
            }

            if (objectType == NullFormatter.FaultReasonDataType)
            {
                return true;
            }
            if (objectType == NullFormatter.RecievedFaultType)
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader.Skip();
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteNull();
        }
    }
}
