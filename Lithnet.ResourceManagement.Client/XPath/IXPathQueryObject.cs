using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// An interface that defines a query predicate
    /// </summary>
    public interface IXPathQueryObject
    {
        /// <summary>
        /// Builds the string representation of the query object
        /// </summary>
        /// <returns>Returns the string representatio of the query object</returns>
        string BuildQueryString();
    }
}
