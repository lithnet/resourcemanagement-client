using System;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public enum RequestFailureSource
    {
        ManagementPolicyRule,
        SystemConstraint,
        Workflow,
        ResourceIsMissing,
        Other
    }
}