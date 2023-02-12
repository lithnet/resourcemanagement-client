using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    [System.Diagnostics.DebuggerStepThrough()]
    [System.CodeDom.Compiler.GeneratedCode("System.ServiceModel", "4.0.0.0")]
    internal partial class NativeSearchClient : System.ServiceModel.ClientBase<ISearch>, ISearch
    {
        public NativeSearchClient()
        {
        }

        public NativeSearchClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public NativeSearchClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public NativeSearchClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public NativeSearchClient(Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public Task<Message> EnumerateAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.EnumerateAsync(request));
        }

        public Task<Message> PullAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.PullAsync(request));
        }

        public Task<Message> RenewAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.RenewAsync(request));
        }

        public Task<Message> GetStatusAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.GetStatusAsync(request));
        }

        public Task<Message> ReleaseAsync(Message request)
        {
            return InternalExtensions.InvokeAsync(this, c => c.ReleaseAsync(request));
        }
    }
}