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
            ConfigureAuthenticatedWsHttpBinding(binding, recieveTimeoutSeconds, sendTimeoutSeconds);
            return binding;
        }

        public static Binding GetNetTcpContextBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            NetTcpContextBinding binding = new NetTcpContextBinding();
            ConfigureNetTcpBindingSettings(binding, recieveTimeoutSeconds, sendTimeoutSeconds);
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
                ConfigureAuthenticatedWsHttpBinding(binding, recieveTimeoutSeconds,sendTimeoutSeconds);
                return binding;
            }
            else
            {
                throw new PlatformNotSupportedException("Unable to create a WSHttpContextBinding on this platform");
            }
        }

        public static Binding GetNetTcpContextBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            if (FrameworkUtilities.IsFramework)
            {
                var assy = Assembly.Load("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                var t = assy.GetType("System.ServiceModel.NetTcpContextBinding", true);
                var binding = (NetTcpBinding)Activator.CreateInstance(t);
                ConfigureNetTcpBindingSettings(binding, recieveTimeoutSeconds, sendTimeoutSeconds);
                return binding;
            }
            else
            {
                throw new PlatformNotSupportedException("Unable to create a NetTcpContextBinding on this platform");
            }
        }
#endif

        public static Binding GetWsAuthenticatedBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            WSHttpBinding binding = new WSHttpBinding();
            ConfigureAuthenticatedWsHttpBinding(binding, recieveTimeoutSeconds, sendTimeoutSeconds);
            return binding;
        }

        public static Binding GetNetTcpBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            NetTcpBinding binding = new NetTcpBinding();
            ConfigureNetTcpBindingSettings(binding, recieveTimeoutSeconds, sendTimeoutSeconds);
            return binding;
        }

        public static Binding GetNetTcpStreamedBinding(int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            NetTcpBinding binding = new NetTcpBinding();
            ConfigureNetTcpBindingSettings(binding, recieveTimeoutSeconds, sendTimeoutSeconds);
            binding.TransferMode = TransferMode.Streamed;
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

        private static void ConfigureAuthenticatedWsHttpBinding(WSHttpBinding binding, int recieveTimeoutSeconds, int sendTimeoutSeconds)
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
        }

        private static void ConfigureNetTcpBindingSettings(NetTcpBinding binding, int recieveTimeoutSeconds, int sendTimeoutSeconds)
        {
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(recieveTimeoutSeconds);
            binding.SendTimeout = TimeSpan.FromSeconds(sendTimeoutSeconds);
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
        }
    }
}
