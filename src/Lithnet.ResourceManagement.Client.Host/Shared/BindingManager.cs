using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Lithnet.ResourceManagement.Client
{
    internal static class BindingManager
    {
#if NETFRAMEWORK
        public static Binding GetWsHttpContextBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            WSHttpContextBinding binding = new WSHttpContextBinding();
            SetAuthenticatedBinding(binding, recieveTimeoutSeconds, sendTimeoutSeconds);
            return binding;
        }
#else
        public static Binding GetWsHttpContextBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            if (FrameworkUtilities.IsFramework)
            {
                var assy = Assembly.Load("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                var t = assy.GetType("System.ServiceModel.WSHttpContextBinding", true);
                var binding = (WSHttpBinding)Activator.CreateInstance(t);
                SetAuthenticatedBinding(binding, recieveTimeoutSeconds,sendTimeoutSeconds);
                return binding;
            }
            else
            {
                throw new PlatformNotSupportedException("Unable to create a WSHttpContextBinding on this platform");
            }
        }
#endif

        public static Binding GetWsAuthenticatedBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            WSHttpBinding binding = new WSHttpBinding();
            SetAuthenticatedBinding(binding, recieveTimeoutSeconds, sendTimeoutSeconds);
            return binding;
        }

        private static Binding SetAuthenticatedBinding(WSHttpBinding binding, int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
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
