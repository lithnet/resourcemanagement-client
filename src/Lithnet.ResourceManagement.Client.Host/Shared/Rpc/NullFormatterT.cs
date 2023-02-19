using System.Diagnostics;
using MessagePack;
using MessagePack.Formatters;
using Newtonsoft.Json;

namespace Lithnet.ResourceManagement.Client
{
    internal class NullFormatter<T> : NullFormatter, IMessagePackFormatter<T>, IFormatterResolver
    {
        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
        {
            Trace.WriteLine($"Serializing {typeof(T)}");
            writer.WriteNil();
        }

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            Trace.WriteLine($"Deserializing {typeof(T)}");
            reader.TryReadNil();
            return default;
        }

        public IMessagePackFormatter<T1> GetFormatter<T1>()
        {
            if (typeof(T1) == typeof(T))
            {
                return (IMessagePackFormatter<T1>)this;
            }

            return null;
        }
    }
}