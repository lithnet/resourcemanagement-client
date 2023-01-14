using System;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Lithnet.ResourceManagement.Client
{
    internal static class BindingManager
    {
        public static Binding GetWsHttpContextBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(recieveTimeoutSeconds);
            binding.SendTimeout = TimeSpan.FromSeconds(sendTimeoutSeconds);
            binding.BypassProxyOnLocal = false;
            binding.TransactionFlow = false;
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.NegotiateServiceCredential = true;
            return binding;
        }

        public static Binding GetWsHttpBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(recieveTimeoutSeconds);
            binding.SendTimeout = TimeSpan.FromSeconds(sendTimeoutSeconds);
            binding.BypassProxyOnLocal = false;
            binding.TransactionFlow = false;
            binding.Security.Mode = SecurityMode.None;
            return binding;
        }
    }
}
