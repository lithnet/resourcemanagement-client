using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    internal static class JsonOptionsFactory
    {
        public const string MetadataService = "Metadata";
        public const string ResourceService = "Resource";
        public const string ResourceFactoryService = "ResourceFactory";
        public const string SearchService = "Search";
        public const string ApprovalService = "Approval";

        public static JsonRpcTargetOptions GetTargetOptions(string serviceName)
        {
            return new JsonRpcTargetOptions() { MethodNameTransform = (x) => $"{serviceName}_{x}" };
        }

        public static JsonRpcProxyOptions GetProxyOptions(string serviceName)
        {
            return new JsonRpcProxyOptions() { MethodNameTransform = (x) => $"{serviceName}_{x}" };
        }
    }
}
