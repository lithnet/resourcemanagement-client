using System;
using System.IO;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;

namespace Lithnet.ResourceManagement.Client.Host
{
    public class RmcJsonRpc : JsonRpc
    {
        public RmcJsonRpc(Stream stream) : base(stream)
        {
        }

        public RmcJsonRpc(IJsonRpcMessageHandler messageHandler) : base(messageHandler)
        {
        }

        public RmcJsonRpc(IJsonRpcMessageHandler messageHandler, object target) : base(messageHandler, target)
        {
        }

        public RmcJsonRpc(Stream sendingStream, Stream receivingStream, object target = null) : base(sendingStream, receivingStream, target)
        {
        }

        protected override JsonRpcError.ErrorDetail CreateErrorDetails(JsonRpcRequest request, Exception exception)
        {
            // We can hook exceptions needing special handling here
            return base.CreateErrorDetails(request, exception);
        }

        protected override bool IsFatalException(Exception ex)
        {
            Logger.LogError(ex, "A client generated an error");
            return base.IsFatalException(ex);
        }
    }
}
