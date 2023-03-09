using System;
using System.Security.Principal;
using Microsoft.Win32;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal static class SettingsProvider
    {
        private const string rmcBaseKey = @"SYSTEM\CurrentControlSet\Services\LithnetRMCProxy";
        private const string mimBaseKey = @"SYSTEM\CurrentControlSet\Services\FIMService";

        public static int MimServicePort { get; private set; } = 5725;

        public static int ProxyServicePort { get; private set; } = 5735;

        public static SecurityIdentifier AuthorizedProxyUsers { get; private set; }

        public static string AuthorizedProxyUsersName { get; private set; }

        static SettingsProvider()
        {
            var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var mimKey = baseKey.OpenSubKey(mimBaseKey);
            var rmcKey = baseKey.OpenSubKey(rmcBaseKey);

            try
            {
                if (int.TryParse(mimKey?.GetValue("ResourceManagementServicePort", "5725")?.ToString(), out int port))
                {
                    MimServicePort = port;
                }
                else if (int.TryParse(rmcKey?.GetValue("ResourceManagementServicePort", "5725")?.ToString(), out port))
                {
                    MimServicePort = port;
                }
                else
                {
                    MimServicePort = 5725;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Unable to resolve MIM service port from registry");
            }

            try
            {
                var sidString = rmcKey?.GetValue("AuthorizedUsers", null) as string;

                if (!string.IsNullOrWhiteSpace(sidString))
                {
                    try
                    {
                        AuthorizedProxyUsers = new SecurityIdentifier(sidString);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"Unable to translate AuthorizedUsers into SID '{sidString}'");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Unable to resolve AuthorizedUsers from registry");
            }

            AuthorizedProxyUsers ??= new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

            try
            {
                AuthorizedProxyUsersName = ((NTAccount)AuthorizedProxyUsers.Translate(typeof(NTAccount))).Value;
            }
            catch
            {
                AuthorizedProxyUsersName = AuthorizedProxyUsers.ToString();
            }

            try
            {
                if (int.TryParse(rmcKey?.GetValue("ProxyPort", "5735")?.ToString(), out int port))
                {
                    ProxyServicePort = port;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Unable to resolve proxy port from registry");
            }
        }
    }
}
