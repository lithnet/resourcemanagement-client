using System;
using System.Runtime.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// A list of permissions that a user can have on an attribute
    /// </summary>
    [Flags]
    [DataContract]
    public enum AttributePermission
    {
        /// <summary>
        /// The user has no permissions or the permissions are unknown
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// The user can add a value to a multi-valued attribute
        /// </summary>
        [EnumMember]
        Add = 1,

        /// <summary>
        /// The user can create a resource
        /// </summary>
        [EnumMember]
        Create = 2,

        /// <summary>
        /// The user can modify a single valued attribute
        /// </summary>
        [EnumMember]
        Modify = 4,

        /// <summary>
        /// The user can delete a resource
        /// </summary>
        [EnumMember]
        Delete = 8,

        /// <summary>
        /// The user can read an attribute
        /// </summary>
        [EnumMember]
        Read = 16,

        /// <summary>
        /// The user can remove values from a multi-valued attribute
        /// </summary>
        [EnumMember]
        Remove = 32
    }
}
