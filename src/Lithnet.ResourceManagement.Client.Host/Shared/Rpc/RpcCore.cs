using System.IO;
using Lithnet.ResourceManagement.Client.Host;
using MessagePack;
using MessagePack.Resolvers;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    internal static class RpcCore
    {
        private static bool UseMessagePack = true;

        public const string PipeNameFormatTemplate = @"\\.\pipe\lithnet\rmc\{0}";

        public static JsonRpcEnumerableSettings JsonRpcEnumerableSettings = new JsonRpcEnumerableSettings { MinBatchSize = 100, MaxReadAhead = 100, Prefetch = 100 };

        public static IJsonRpcMessageHandler GetMessageHandler(Stream pipe)
        {
            return UseMessagePack ? GetMessagePackMessageHandler(pipe) : GetJsonMessageHandler(pipe);
        }

        private static IJsonRpcMessageHandler GetMessagePackMessageHandler(Stream pipe)
        {
            var messageFormatter = new MessagePackFormatter();
            var options = MessagePackFormatter.DefaultUserDataSerializationOptions
            .WithResolver(
                CompositeResolver.Create(new IFormatterResolver[] {
                    new MessageSerializer(),
                    StandardResolver.Instance
            }));

            messageFormatter.SetMessagePackSerializerOptions(options);
            return new LengthHeaderMessageHandler(pipe, pipe, messageFormatter);
        }

        private static IJsonRpcMessageHandler GetJsonMessageHandler(Stream pipe)
        {
            var messageFormatter = new JsonMessageFormatter();
            messageFormatter.JsonSerializer.Converters.Add(new MessageSerializer());

            return new LengthHeaderMessageHandler(pipe, pipe, messageFormatter);
        }
    }
}
