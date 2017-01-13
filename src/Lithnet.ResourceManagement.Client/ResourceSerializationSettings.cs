using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines the settings that control the serialization of resources
    /// </summary>
    public struct ResourceSerializationSettings
    {
        /// <summary>
        /// Gets or sets a value that indicates how attribute values should be serialized
        /// </summary>
        public AttributeValueSerializationHandling ValueFormat { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates which attributes should be represented as arrays
        /// </summary>
        public ArraySerializationHandling ArrayHandling { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if permission hints should be included in the serialized resource
        /// </summary>
        public bool IncludePermissionHints { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if null attributes should be included in the response. If the IncludePermissionHints value is set, null values will always be included.
        /// </summary>
        public bool IncludeNullValues { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the format that the resource should be serialized in
        /// </summary>
        public ResourceSerializationHandling ResourceFormat { get; set; }
    }
}
