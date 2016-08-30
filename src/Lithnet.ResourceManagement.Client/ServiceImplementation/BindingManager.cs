using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Lithnet.ResourceManagement.Client
{
    internal static class BindingManager
    {
        public static Binding GetWsHttpContextBinding()
        {
            WSHttpContextBinding binding = new WSHttpContextBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(ResourceManagementClient.Configuration.ReceiveTimeoutSeconds);
            binding.SendTimeout = TimeSpan.FromSeconds(ResourceManagementClient.Configuration.SendTimeoutSeconds);
            binding.BypassProxyOnLocal = false;
            binding.TransactionFlow = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;

            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.NegotiateServiceCredential = true;

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
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

            binding.Security.Mode = SecurityMode.None;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.NegotiateServiceCredential = true;

            return binding;
        }
    }
}
