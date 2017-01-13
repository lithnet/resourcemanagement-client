using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines the ways that attribute values can be serialized
    /// </summary>
    public enum AttributeValueSerializationHandling
    {
        /// <summary>
        /// Values are converted to a primative serializable type (eg string, boolean, integer)
        /// </summary>
        Default = 0,

        /// <summary>
        /// All value types are converted to strings
        /// </summary>
        ConvertToString = 1
    }
}
