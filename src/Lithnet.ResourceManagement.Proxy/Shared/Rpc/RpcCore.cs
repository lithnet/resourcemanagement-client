using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Principal;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Proxy;
using MessagePack;
using MessagePack.Resolvers;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    public static class RpcCore
    {
        internal const byte Ack = 0xFF;

        internal const byte ErrorServer = 0xEF;
        internal const byte ErrorClient = 0xEE;
        internal const byte ErrorVersionMismatch = 0xED;
        internal const byte ErrorAccessDenied = 0xE5;

        internal const byte MessageClientHello = 0xD0;
        internal const byte MessageClientVersionExchange = 0xD1;
        internal const byte MessageClientPostAuthInitialization = 0xD2;

        internal const byte ClientVersion = 0x1;
        internal const byte ServerVersion = 0x1;

        private const bool UseMessagePack = true;

        public const string PipeNameFormatTemplate = @"\\.\pipe\lithnet\rmc\{0}";

        public static JsonRpcEnumerableSettings JsonRpcEnumerableSettings = new JsonRpcEnumerableSettings { MinBatchSize = 100, MaxReadAhead = 100, Prefetch = 100 };

        public static async Task<NegotiateStream> GetServerNegotiateStreamAsync(Stream stream, TokenImpersonationLevel requestedImpersonationLevel)
        {
            return await GetNegotiateStreamAsync(stream, (NetworkCredential)CredentialCache.DefaultCredentials, true, null, requestedImpersonationLevel);
        }

        public static async Task<NegotiateStream> GetClientNegotiateStreamAsync(Stream stream, NetworkCredential credentials, string serverSpn, TokenImpersonationLevel requestedImpersonationLevel)
        {
            return await GetNegotiateStreamAsync(stream, credentials, false, serverSpn, requestedImpersonationLevel);
        }

        private static async Task<NegotiateStream> GetNegotiateStreamAsync(Stream networkStream, NetworkCredential credentials, bool isServer, string serverSpn, TokenImpersonationLevel requestedImpersonationLevel)
        {
            NegotiateStream authenticatedStream = new NegotiateStream(networkStream, false);

            if (isServer)
            {
                await authenticatedStream.AuthenticateAsServerAsync(credentials, ProtectionLevel.EncryptAndSign, requestedImpersonationLevel);
            }
            else
            {
                await authenticatedStream.AuthenticateAsClientAsync(credentials, serverSpn, ProtectionLevel.EncryptAndSign, requestedImpersonationLevel);
            }

            if (!authenticatedStream.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("The negotiate stream was not authenticated");
            }

            return authenticatedStream;
        }

        public static IJsonRpcMessageHandler GetMessageHandler(Stream sendingStream, Stream receivingStream)
        {
            return UseMessagePack ? GetMessagePackMessageHandler(sendingStream, receivingStream) : GetJsonMessageHandler(sendingStream, receivingStream);
        }

        private static IJsonRpcMessageHandler GetMessagePackMessageHandler(Stream sendingStream, Stream receivingStream)
        {
            var messageFormatter = new MessagePackFormatter();
            var options = MessagePackFormatter.DefaultUserDataSerializationOptions
            .WithResolver(
                CompositeResolver.Create(new IFormatterResolver[] {
                    new MessageSerializer(),
                    NullFormatter.GetInstance(NullFormatter.FaultCodeDataType),
                    NullFormatter.GetInstance(NullFormatter.RecievedFaultType),
                    NullFormatter.GetInstance(NullFormatter.FaultReasonDataType),
                    StandardResolver.Instance,
            }));

            messageFormatter.SetMessagePackSerializerOptions(options);
            return new LengthHeaderMessageHandler(sendingStream, receivingStream, messageFormatter);
        }

        private static IJsonRpcMessageHandler GetJsonMessageHandler(Stream sendingStream, Stream receivingStream)
        {
            var messageFormatter = new JsonMessageFormatter();
            messageFormatter.JsonSerializer.Converters.Add(new MessageSerializer());
            messageFormatter.JsonSerializer.Converters.Add(new JsonNullConverter());

            return new LengthHeaderMessageHandler(sendingStream, receivingStream, messageFormatter);
        }
    }
}
