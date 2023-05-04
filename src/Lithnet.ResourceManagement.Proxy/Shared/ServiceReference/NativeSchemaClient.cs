using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    [DebuggerStepThrough()]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    internal partial class NativeSchemaClient : System.ServiceModel.ClientBase<IMetadataExchange>, IMetadataExchange
    {
        public NativeSchemaClient()
        {
        }

        public NativeSchemaClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public NativeSchemaClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public NativeSchemaClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public NativeSchemaClient(Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public Task<Message> GetAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.GetAsync(request));
        }
    }
}