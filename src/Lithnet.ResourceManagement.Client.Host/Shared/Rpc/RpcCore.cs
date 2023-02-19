using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Principal;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.Host;
using MessagePack;
using MessagePack.Resolvers;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    public static class RpcCore
    {
        internal const byte ServerAck = 1;
        internal const byte ServerError = 2;
        internal const byte ClientInitialization = 100;

        private static bool UseMessagePack = true;

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
            NegotiateStream authenticatedStream = new NegotiateStream(networkStream, true);

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
