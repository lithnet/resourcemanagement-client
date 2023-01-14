using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    [DebuggerStepThrough()]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    internal partial class NativeResourceFactoryClient : System.ServiceModel.ClientBase<IResourceFactory>, IResourceFactory
    {
        public NativeResourceFactoryClient()
        {
        }

        public NativeResourceFactoryClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public NativeResourceFactoryClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public NativeResourceFactoryClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public NativeResourceFactoryClient(Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public Task<Message> CreateAsync(Message request)
        {
            return this.Channel.CreateAsync(request);
        }
    }
}