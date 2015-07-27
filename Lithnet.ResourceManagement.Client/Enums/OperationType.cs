using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Defines the type of operation in progress on an object
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// No operation has been specified
        /// </summary>
        None = 0,

        /// <summary>
        /// A create operation
        /// </summary>
        Create = 1,

        /// <summary>
        /// An update operation
        /// </summary>
        Update = 2,

        /// <summary>
        /// A delete operation
        /// </summary>
        Delete = 3
    }
}
