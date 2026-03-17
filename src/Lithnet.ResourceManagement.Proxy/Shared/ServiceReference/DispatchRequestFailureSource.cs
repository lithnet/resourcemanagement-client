using System;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public enum DispatchRequestFailureSource
    {
        ServiceIsStopping,
        IdentityIsNotFound,
        AlternateEndpointNotSupported,
        Other,
        InvalidResourceIdentifier,
        Synchronization,
        ActionProcessor
    }
}