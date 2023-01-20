using System;
using System.Collections.Generic;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    internal static class FrameworkUtilities
    {
        public static bool IsFramework { get; } = typeof(object).Assembly.FullName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase);
    }
}
