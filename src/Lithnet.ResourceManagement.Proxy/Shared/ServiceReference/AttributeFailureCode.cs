using System;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public enum AttributeFailureCode
    {
        ChangeOperationIsNotSupported,
        ValueViolatesRegularExpression,
        ValueViolatesDataTypeFormat,
        RequiredValueIsMissing,
        ReferencedResourceIsMissing,
        AttributeNameViolatesSchema,
        ResourceTypeViolatesSchema,
        ResourceTypeIsMissing,
        SystemAttributeIsReadOnly,
        ValueViolatesUniqueness,
        Other,
        None
    }
}