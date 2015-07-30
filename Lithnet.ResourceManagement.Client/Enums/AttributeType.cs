using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines an attributeName data type
    /// </summary>
    public enum AttributeType
    {
        /// <summary>
        /// An unknown or unspecified data type
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A binary data type
        /// </summary>
        Binary = 1,

        /// <summary>
        /// A boolean data type
        /// </summary>
        Boolean = 2,

        /// <summary>
        /// A DateTime data type
        /// </summary>
        DateTime = 3,

        /// <summary>
        /// An integer data type
        /// </summary>
        Integer = 4,

        /// <summary>
        /// A reference data type
        /// </summary>
        Reference = 5,

        /// <summary>
        /// A string data type
        /// </summary>
        String = 6,

        /// <summary>
        /// A text data type
        /// </summary>
        Text = 7
    }
}
