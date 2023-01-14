using System.IO;
using Lithnet.ResourceManagement.Client.Host;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    public static class RpcCore
    {
        public const string PipeNameFormatTemplate = @"\\.\pipe\lithnet\rmc\{0}";

        public static JsonRpcEnumerableSettings JsonRpcEnumerableSettings = new JsonRpcEnumerableSettings { MinBatchSize = 100, MaxReadAhead = 100, Prefetch = 100 };

        public static IJsonRpcMessageHandler GetMessageHandler(Stream pipe)
        {
            var messageFormatter = new JsonMessageFormatter();
            messageFormatter.JsonSerializer.Converters.Add(new MessageSerializer());

            return new LengthHeaderMessageHandler(pipe, pipe, messageFormatter);
        }

    }
}
