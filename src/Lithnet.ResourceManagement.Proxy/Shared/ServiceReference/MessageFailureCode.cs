using System;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public enum MessageFailureCode
    {
        DialectIsNotSupported,
        RequestMessageViolatesProtocol,
        CultureIsNotEnabled,
        CultureIsNotSupportedOnOperation,
        Other,
        None
    }
}