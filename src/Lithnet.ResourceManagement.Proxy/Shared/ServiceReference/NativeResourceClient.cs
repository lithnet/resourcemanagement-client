using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    [DebuggerStepThrough()]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    internal partial class NativeResourceClient : System.ServiceModel.ClientBase<IResource>, IResource
    {
        public NativeResourceClient()
        {
        }

        public NativeResourceClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public NativeResourceClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public NativeResourceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public NativeResourceClient(Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public Task<Message> GetAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.GetAsync(request));
        }
        
        public Task<Message> PutAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.PutAsync(request));
        }

        public Task<Message> DeleteAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.DeleteAsync(request));
        }
    }
}