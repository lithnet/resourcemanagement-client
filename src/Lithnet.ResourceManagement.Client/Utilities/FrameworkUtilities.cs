using System;

namespace Lithnet.ResourceManagement.Client
{
    internal static class FrameworkUtilities
    {
        public static bool IsFramework { get; } = typeof(object).Assembly.FullName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase);

        public static bool IsFrameworkCompiled { get; } =
#if NETFRAMEWORK
            true;
#else
            false;
#endif
    }
}
