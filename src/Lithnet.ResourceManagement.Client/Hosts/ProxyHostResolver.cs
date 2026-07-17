using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The single place that decides which on-disk proxy host executable to run. Detection and
    /// launch must agree, so both consult this resolver.
    /// </summary>
    /// <remarks>
    /// Expected error conditions: a host location that was explicitly configured (the RmcHostExe
    /// option, RmcConfiguration.FxHostPath, an FxHostPath registry value, or the installer-written
    /// HostPath registry value) but names a missing file indicates a misconfiguration or a broken
    /// install, and throws FileNotFoundException naming the source of the configuration. Silently
    /// extracting the embedded copy instead would mask the broken install, and on an
    /// application-control (WDAC/AppLocker) machine the extracted copy is blocked anyway, producing
    /// a far less actionable error. The opportunistic assembly-directory probe is not
    /// configuration, so a miss there simply falls through. No installed host at all returns null,
    /// and the caller uses the embedded copy.
    /// </remarks>
    internal static class ProxyHostResolver
    {
        private const string HostRegistryKey = @"SOFTWARE\Lithnet\Resource Management Client";
        private const string HostRegistryValue = "HostPath";
        private const string FxHostPathKey = @"Software\Lithnet\ResourceManagementClient";
        private const string FxHostPathValue = "FxHostPath";
        internal const string HostExeName = "Lithnet.ResourceManagement.Proxy.exe";

        internal static Func<string> RegistryHostPathProvider = ReadRegistryHostPath;
        internal static Func<string> UserFxHostPathProvider = ReadUserFxHostPath;
        internal static Func<string> MachineFxHostPathProvider = ReadMachineFxHostPath;

        public static string ResolveInstalledHost(ResourceManagementClientOptions p)
        {
            if (!string.IsNullOrWhiteSpace(p.RmcHostExe))
            {
                return RequireExists(p.RmcHostExe, "the RmcHostExe option");
            }

            string configuredDirectory = RmcConfiguration.FxHostPath;

            if (!string.IsNullOrWhiteSpace(configuredDirectory))
            {
                return RequireExists(Path.Combine(configuredDirectory, HostExeName), "the RmcConfiguration.FxHostPath setting");
            }

            string userFxHostPath = UserFxHostPathProvider();

            if (!string.IsNullOrWhiteSpace(userFxHostPath))
            {
                return RequireExists(Path.Combine(userFxHostPath, HostExeName), $@"the registry value HKCU\{FxHostPathKey}\{FxHostPathValue}");
            }

            string machineFxHostPath = MachineFxHostPathProvider();

            if (!string.IsNullOrWhiteSpace(machineFxHostPath))
            {
                return RequireExists(Path.Combine(machineFxHostPath, HostExeName), $@"the registry value HKLM\{FxHostPathKey}\{FxHostPathValue}");
            }

            string installedPath = RegistryHostPathProvider();

            if (!string.IsNullOrWhiteSpace(installedPath))
            {
                return RequireExists(installedPath, $@"the registry value HKLM\{HostRegistryKey}\{HostRegistryValue}");
            }

            string probed = ProbeAssemblyDirectories();

            if (probed != null)
            {
                return probed;
            }

            Trace.WriteLine("No installed proxy host was found. The embedded host will be used.");
            return null;
        }

        private static string ProbeAssemblyDirectories()
        {
            List<string> probePaths = new List<string>()
            {
                GetParentPath(Assembly.GetExecutingAssembly()),
                GetParentPath(Assembly.GetCallingAssembly()),
                GetParentPath(Assembly.GetEntryAssembly())
            };

            foreach (string probePath in probePaths)
            {
                if (string.IsNullOrWhiteSpace(probePath))
                {
                    continue;
                }

                string fxHostExe = Path.Combine(probePath, "fxhost", HostExeName);

                if (File.Exists(fxHostExe))
                {
                    Trace.WriteLine($"Found proxy host alongside the assembly at {fxHostExe}");
                    return fxHostExe;
                }

                string siblingExe = Path.Combine(probePath, HostExeName);

                if (File.Exists(siblingExe))
                {
                    Trace.WriteLine($"Found proxy host alongside the assembly at {siblingExe}");
                    return siblingExe;
                }
            }

            return null;
        }

        private static string GetParentPath(Assembly assembly)
        {
            if (assembly == null || string.IsNullOrWhiteSpace(assembly.Location))
            {
                return null;
            }

            return Path.GetDirectoryName(assembly.Location);
        }

        internal static string ReadUserFxHostPath()
        {
            return ReadRegistryValue(RegistryHive.CurrentUser, FxHostPathKey, FxHostPathValue);
        }

        internal static string ReadMachineFxHostPath()
        {
            return ReadRegistryValue(RegistryHive.LocalMachine, FxHostPathKey, FxHostPathValue);
        }

        internal static string ReadRegistryHostPath()
        {
            return ReadRegistryValue(RegistryHive.LocalMachine, HostRegistryKey, HostRegistryValue);
        }

        private static string ReadRegistryValue(RegistryHive hive, string keyName, string valueName)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return null;
            }

            try
            {
                // The 64-bit view is read explicitly so a 32-bit consumer is not redirected to
                // Wow6432Node, matching where the installer writes the value.
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64))
                using (RegistryKey key = baseKey.OpenSubKey(keyName))
                {
                    if (key == null)
                    {
                        return null;
                    }

                    return key.GetValue(valueName, null) as string;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($@"Unable to read {valueName} from {hive}\{keyName}: {ex}");
                return null;
            }
        }

        private static string RequireExists(string path, string source)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"The Resource Management Client proxy host was configured by {source}, but no file exists at '{path}'. " +
                    "Correct the configuration, or repair the Lithnet Resource Management Client Proxy installation.",
                    path);
            }

            Trace.WriteLine($"Using installed proxy host at {path} (from {source})");
            return path;
        }
    }
}
