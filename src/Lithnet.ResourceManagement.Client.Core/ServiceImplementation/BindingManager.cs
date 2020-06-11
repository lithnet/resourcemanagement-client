using System;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Lithnet.ResourceManagement.Client
{
    internal static class BindingManager
    {
        public static Binding GetWsHttpContextBinding()
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(ResourceManagementClient.Configuration.ReceiveTimeoutSeconds);
            binding.SendTimeout = TimeSpan.FromSeconds(ResourceManagementClient.Configuration.SendTimeoutSeconds);
            binding.BypassProxyOnLocal = false;
            binding.TransactionFlow = false;
           // binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

            binding.Security.Mode = SecurityMode.Transport;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            //binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            //binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;

            //binding.Security.Message.EstablishSecurityContext = false;
            //binding.Security.Message.NegotiateServiceCredential = true;

            return binding;
        }

        public static Binding GetWsHttpBinding()
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(ResourceManagementClient.Configuration.ReceiveTimeoutSeconds);
            binding.SendTimeout = TimeSpan.FromSeconds(ResourceManagementClient.Configuration.SendTimeoutSeconds);
            binding.BypassProxyOnLocal = false;
            binding.TransactionFlow = false;
            //binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

            binding.Security.Mode = SecurityMode.None;

            return binding;
        }
    }
}
