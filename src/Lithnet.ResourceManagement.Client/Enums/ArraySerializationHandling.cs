using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines the ways that arrays are serialized
    /// </summary>
    public enum ArraySerializationHandling
    {
        /// <summary>
        /// All multivalued attributes are serialized as arrays
        /// </summary>
        AllMultivaluedAttributes = 0,

        /// <summary>
        /// All attributes are serialized as arrays
        /// </summary>
        AllAttributes = 1,

        /// <summary>
        /// Multivalued attributes with more than a single value are serialized as arrays. Multivalued attributes with zero or one value are not serialized in an array.
        /// </summary>
        WhenRequired = 2,
    }
}
