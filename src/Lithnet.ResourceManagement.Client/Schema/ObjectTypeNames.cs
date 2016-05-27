using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines a list of built-in object type names in the Resource Management Service
    /// </summary>
    /// <remarks>
    /// The constants defined is this class can be used in place of manually specifying the names of the object types as strings in code
    /// </remarks>
    public static class ObjectTypeNames
    {
        /// <summary>
        /// The person object type
        /// </summary>
        public const string Person = "Person";

        /// <summary>
        /// The Set object type
        /// </summary>
        public const string Set = "Set";

        /// <summary>
        /// The ManagementPolicyRule object type
        /// </summary>
        public const string ManagementPolicyRule = "ManagementPolicyRule";

        /// <summary>
        /// The WorkflowDefinition object type
        /// </summary>
        public const string Workflow = "WorkflowDefinition";

        /// <summary>
        /// The Group object type
        /// </summary>
        public const string Group = "Group";

        /// <summary>
        /// The Approval object type
        /// </summary>
        public const string Approval = "Approval";

        /// <summary>
        /// The ApprovalResponse object type
        /// </summary>
        public const string ApprovalResponse = "ApprovalResponse";

        /// <summary>
        /// The AttributeTypeDescription object type
        /// </summary>
        public const string AttributeTypeDescription = "AttributeTypeDescription";

        /// <summary>
        /// The ObjectTypeDescription object type
        /// </summary>
        public const string ObjectTypeDescription = "ObjectTypeDescription";

        /// <summary>
        /// The BindingDescription object type
        /// </summary>
        public const string BindingDescription = "BindingDescription";
    }
}

