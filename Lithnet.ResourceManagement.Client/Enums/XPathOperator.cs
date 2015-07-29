using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public enum XPathOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals,
        IsPresent,
        IsNotPresent,
        Contains,
        StartsWith,
        EndsWith
    }
}
