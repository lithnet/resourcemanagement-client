using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
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
        private static bool UseMessagePack = true;

        public const string PipeNameFormatTemplate = @"\\.\pipe\lithnet\rmc\{0}";

        public static JsonRpcEnumerableSettings JsonRpcEnumerableSettings = new JsonRpcEnumerableSettings { MinBatchSize = 100, MaxReadAhead = 100, Prefetch = 100 };

        public static async Task<NegotiateStream> GetServerNegotiateStreamAsync(Stream stream)
        {
            return await GetNegotiateStreamAsync(stream, (NetworkCredential)CredentialCache.DefaultCredentials, true, null);
        }

        public static async Task<NegotiateStream> GetClientNegotiateStreamAsync(Stream stream, NetworkCredential credentials, string serverSpn)
        {
            return await GetNegotiateStreamAsync(stream, credentials, false, serverSpn);
        }

        private static async Task<NegotiateStream> GetNegotiateStreamAsync(Stream networkStream, NetworkCredential credentials, bool isServer, string serverSpn)
        {
            NegotiateStream authenticatedStream = new NegotiateStream(networkStream, true);

            if (isServer)
            {
                await authenticatedStream.AuthenticateAsServerAsync(credentials, ProtectionLevel.EncryptAndSign, System.Security.Principal.TokenImpersonationLevel.Identification);
            }
            else
            {
                var impersonationLevel = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? TokenImpersonationLevel.Impersonation : TokenImpersonationLevel.Identification;

                await authenticatedStream.AuthenticateAsClientAsync(credentials, serverSpn, ProtectionLevel.EncryptAndSign, impersonationLevel);
            }

            if (!authenticatedStream.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("The negotiate stream was not authenticated");
            }

            return authenticatedStream;
        }

        public static IJsonRpcMessageHandler GetMessageHandler(Stream stream)
        {
            return UseMessagePack ? GetMessagePackMessageHandler(stream) : GetJsonMessageHandler(stream);
        }

        private static IJsonRpcMessageHandler GetMessagePackMessageHandler(Stream stream)
        {
            var messageFormatter = new MessagePackFormatter();
            var options = MessagePackFormatter.DefaultUserDataSerializationOptions
            .WithResolver(
                CompositeResolver.Create(new IFormatterResolver[] {
                    new MessageSerializer(),
                    StandardResolver.Instance,
            }));

            messageFormatter.SetMessagePackSerializerOptions(options);
            return new LengthHeaderMessageHandler(stream, stream, messageFormatter);
        }

        private static IJsonRpcMessageHandler GetJsonMessageHandler(Stream stream)
        {
            var messageFormatter = new JsonMessageFormatter();
            messageFormatter.JsonSerializer.Converters.Add(new MessageSerializer());

            return new LengthHeaderMessageHandler(stream, stream, messageFormatter);
        }
    }
}
