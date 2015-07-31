using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The type of boolean operator to apply to an XPath query gruop
    /// </summary>
    public enum GroupOperator
    {
        /// <summary>
        /// The boolean 'and' operator
        /// </summary>
        And,

        /// <summary>
        /// The boolean 'or' operator
        /// </summary>
        Or
    }
}
