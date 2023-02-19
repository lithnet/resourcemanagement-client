using System;
using MessagePack;

namespace Lithnet.ResourceManagement.Client
{
    internal class NullFormatter
    {
        internal static readonly Type FaultCodeDataType = typeof(System.ServiceModel.FaultException).Assembly.GetType("System.ServiceModel.FaultException+FaultCodeData", throwOnError: true);

        internal static readonly Type FaultReasonDataType = typeof(System.ServiceModel.FaultException).Assembly.GetType("System.ServiceModel.FaultException+FaultReasonData", throwOnError: true);

        internal static readonly Type RecievedFaultType = typeof(System.ServiceModel.Channels.MessageFault).Assembly.GetType("System.ServiceModel.Channels.ReceivedFault", throwOnError: true);

        public static IFormatterResolver GetInstance(Type type)
        {
            Type generic = typeof(NullFormatter<>);
            Type constructed = generic.MakeGenericType(type);
            return (IFormatterResolver)Activator.CreateInstance(constructed);
        }
    }
}