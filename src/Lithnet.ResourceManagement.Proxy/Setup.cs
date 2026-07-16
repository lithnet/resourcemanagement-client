using System;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.System.Services;

namespace Lithnet.ResourceManagement.Proxy
{
    /// <summary>
    /// Code-driven Windows service registration, invoked from the installer via the
    /// "/installservice" and "/uninstallservice" command-line verbs, and usable directly by
    /// administrators.
    ///
    /// The service lifecycle is handled here rather than by the MSI's native
    /// ServiceInstall/ServiceControl tables because the service is optional (workstations install
    /// only the proxy executable for local use), and the Windows Installer ServiceInstall table
    /// cannot make a service conditional without also skipping the executable that is its
    /// component keypath. This mirrors the approach used by Lithnet Access Manager and AutoSync,
    /// using CsWin32-generated P/Invoke.
    ///
    /// The service runs as NT AUTHORITY\NetworkService. It needs no privileged identity of its
    /// own: every MIM operation is performed under the impersonated caller (or the caller's
    /// explicitly supplied credentials), and clients authenticate the service via the host
    /// computer account's SPN.
    ///
    /// Authorization is granted through a local group rather than a hand-edited SID: install
    /// creates the group, seeds it with the local Administrators group so out-of-box behaviour
    /// matches the proxy's built-in default, and writes the group's SID to the AuthorizedUsers
    /// registry value that SettingsProvider reads. Administrators grant access by adding users to
    /// the group.
    ///
    /// Expected error conditions: service creation, group creation, and the registry write are
    /// mandatory for a working install and throw on failure, failing the installer with the cause
    /// in the Application event log (msiexec does not surface console output from deferred custom
    /// actions). Event source registration and service start are best-effort: their failure does
    /// not invalidate the installation, and each failure is logged.
    /// </summary>
    public static class Setup
    {
        private const string ServiceName = "LithnetRMCProxy";
        private const string ServiceDisplayName = "Lithnet Resource Management Proxy";
        private const string ServiceDescription = "Provides support for .NET Core and non-Windows clients accessing the MIM service";
        private const string ServiceDependencies = "FIMService\0\0";
        private const string ServiceAccount = @"NT AUTHORITY\NetworkService";

        private const string AuthorizedUsersGroupName = "Lithnet RMC Proxy Users";
        private const string AuthorizedUsersGroupDescription = "Members of this group are authorized to connect to the Lithnet Resource Management Proxy service";
        private const string ServiceRegistryKey = @"SYSTEM\CurrentControlSet\Services\" + ServiceName;
        private const string AuthorizedUsersValueName = "AuthorizedUsers";

        private const int ERROR_SERVICE_DOES_NOT_EXIST = 1060;

        public static void Install()
        {
            try
            {
                InstallService();

                SecurityIdentifier groupSid = EnsureAuthorizationGroup();
                WriteAuthorizedUsersValue(groupSid);

                RegisterEventLogSources();
                TryStartService();
            }
            catch (Exception ex)
            {
                WriteSetupError($"The {ServiceName} service could not be installed\r\n{ex}");
                throw;
            }
        }

        public static void Uninstall()
        {
            try
            {
                TryStopService();
                DeleteService();

                // The authorization group is deliberately left behind: its membership is
                // administrator-managed access policy, and a reinstall or upgrade must not
                // silently discard it.
            }
            catch (Exception ex)
            {
                WriteSetupError($"The {ServiceName} service could not be uninstalled\r\n{ex}");
                throw;
            }
        }

        private static unsafe void InstallService()
        {
            string binaryPath = "\"" + Process.GetCurrentProcess().MainModule.FileName + "\" /service";

            Logger.LogInfo("Opening service control manager");
            using var serviceManager = PInvoke.OpenSCManager((string)null, null, PInvoke.SC_MANAGER_ALL_ACCESS);
            if (serviceManager.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            Logger.LogInfo($"Checking for existing {ServiceName} service");
            using var serviceHandle = PInvoke.OpenService(serviceManager, ServiceName, PInvoke.SERVICE_ALL_ACCESS);

            if (serviceHandle.IsInvalid)
            {
                int err = Marshal.GetLastWin32Error();
                if (err != ERROR_SERVICE_DOES_NOT_EXIST)
                {
                    throw new Win32Exception(err);
                }

                Logger.LogInfo($"Creating service {ServiceName} as {ServiceAccount} at {binaryPath}");

                // lpdwTagId must be NULL: a tag is only valid for a driver service with a
                // boot/system start type that belongs to a load-ordering group. Requesting a tag
                // for this auto-start SERVICE_WIN32_OWN_PROCESS is an invalid combination and
                // CreateService fails with ERROR_INVALID_PARAMETER. Use the CsWin32 overload that
                // omits lpdwTagId; do not switch to the "out uint lpdwTagId" overload.
                using var newService = PInvoke.CreateService(serviceManager, ServiceName, ServiceDisplayName, PInvoke.SERVICE_ALL_ACCESS, ENUM_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS, SERVICE_START_TYPE.SERVICE_AUTO_START, SERVICE_ERROR.SERVICE_ERROR_NORMAL, binaryPath, null, ServiceDependencies, ServiceAccount, null);

                if (newService.IsInvalid)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                Logger.LogInfo($"Created {ServiceName} service");
                SetDescription(newService);
            }
            else
            {
                Logger.LogInfo($"Found existing {ServiceName} service, updating its configuration");

                // As with CreateService above, use the overload that omits lpdwTagId.
                if (!PInvoke.ChangeServiceConfig(serviceHandle, ENUM_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS, SERVICE_START_TYPE.SERVICE_AUTO_START, SERVICE_ERROR.SERVICE_ERROR_NORMAL, binaryPath, null, ServiceDependencies, ServiceAccount, null, ServiceDisplayName))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                Logger.LogInfo($"Updated {ServiceName} service configuration");
                SetDescription(serviceHandle);
            }
        }

        private static unsafe void SetDescription(System.Runtime.InteropServices.SafeHandle serviceHandle)
        {
            fixed (char* description = ServiceDescription)
            {
                var info = new SERVICE_DESCRIPTIONW { lpDescription = new Windows.Win32.Foundation.PWSTR(description) };
                if (!PInvoke.ChangeServiceConfig2W(serviceHandle, SERVICE_CONFIG.SERVICE_CONFIG_DESCRIPTION, &info))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private static void DeleteService()
        {
            Logger.LogInfo("Opening service control manager");
            using var serviceManager = PInvoke.OpenSCManager((string)null, null, PInvoke.SC_MANAGER_ALL_ACCESS);
            if (serviceManager.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            using var serviceHandle = PInvoke.OpenService(serviceManager, ServiceName, PInvoke.SERVICE_ALL_ACCESS);
            if (serviceHandle.IsInvalid)
            {
                int err = Marshal.GetLastWin32Error();
                if (err == ERROR_SERVICE_DOES_NOT_EXIST)
                {
                    Logger.LogInfo($"The {ServiceName} service was not present");
                    return;
                }

                throw new Win32Exception(err);
            }

            if (!PInvoke.DeleteService(serviceHandle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            Logger.LogInfo($"Deleted the {ServiceName} service");
        }

        /// <summary>
        /// Creates the local authorization group if it does not exist, seeding it with the local
        /// Administrators group so a fresh install authorizes the same principals as the proxy's
        /// built-in default. An existing group is left untouched: its membership is
        /// administrator-managed access policy.
        /// </summary>
        private static SecurityIdentifier EnsureAuthorizationGroup()
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                GroupPrincipal group = GroupPrincipal.FindByIdentity(context, IdentityType.Name, AuthorizedUsersGroupName);

                if (group != null)
                {
                    using (group)
                    {
                        Logger.LogInfo($"The '{AuthorizedUsersGroupName}' group already exists");
                        return group.Sid;
                    }
                }

                Logger.LogInfo($"Creating the '{AuthorizedUsersGroupName}' local group");

                using (GroupPrincipal newGroup = new GroupPrincipal(context, AuthorizedUsersGroupName))
                {
                    newGroup.Description = AuthorizedUsersGroupDescription;
                    newGroup.Save();

                    try
                    {
                        SecurityIdentifier adminsSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

                        using (GroupPrincipal admins = GroupPrincipal.FindByIdentity(context, IdentityType.Sid, adminsSid.Value))
                        {
                            newGroup.Members.Add(admins);
                            newGroup.Save();
                        }

                        Logger.LogInfo($"Seeded the '{AuthorizedUsersGroupName}' group with the local Administrators group");
                    }
                    catch (Exception ex)
                    {
                        // The group exists and is written to the registry, so the install is
                        // functional; only the convenience seeding failed. Log it so an admin
                        // knows the group starts empty.
                        Logger.LogWarning($"The '{AuthorizedUsersGroupName}' group was created, but could not be seeded with the local Administrators group. Add authorized users to the group manually.\r\n{ex}");
                    }

                    return newGroup.Sid;
                }
            }
        }

        private static void WriteAuthorizedUsersValue(SecurityIdentifier groupSid)
        {
            Logger.LogInfo($"Setting {AuthorizedUsersValueName} to '{groupSid.Value}' ({AuthorizedUsersGroupName})");

            using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (RegistryKey key = baseKey.OpenSubKey(ServiceRegistryKey, true))
            {
                if (key == null)
                {
                    throw new InvalidOperationException($"The service registry key {ServiceRegistryKey} was not found. The service must be created before its settings can be written");
                }

                key.SetValue(AuthorizedUsersValueName, groupSid.Value, RegistryValueKind.String);
            }
        }

        /// <summary>
        /// Registers the event sources the service writes to: the ServiceBase automatic
        /// start/stop events (source = service name) and the proxy's own Logger source. The
        /// service account cannot create event sources at runtime, so they must be registered
        /// here, while the installer is elevated. Best-effort: missing sources degrade logging,
        /// not function.
        /// </summary>
        private static void RegisterEventLogSources()
        {
            try
            {
                if (!EventLog.SourceExists(ServiceName))
                {
                    Logger.LogInfo($"Registering the '{ServiceName}' event log source");
                    EventLog.CreateEventSource(ServiceName, "Application");
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Could not register the '{ServiceName}' event log source. Service start and stop events will not be logged.\r\n{ex}");
            }

            try
            {
                Logger.SetupEventSource();
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Could not register the proxy's event log source\r\n{ex}");
            }
        }

        private static void TryStartService()
        {
            try
            {
                Logger.LogInfo($"Starting the {ServiceName} service");

                using (ServiceController controller = new ServiceController(ServiceName))
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                }

                Logger.LogInfo($"The {ServiceName} service is running");
            }
            catch (Exception ex)
            {
                // A start failure does not invalidate the installation (for example the FIM
                // service dependency may itself still be starting); the service is auto-start and
                // will start with the machine.
                Logger.LogWarning($"The {ServiceName} service was installed but could not be started. Start it manually or restart the machine.\r\n{ex}");
            }
        }

        private static void TryStopService()
        {
            try
            {
                using (ServiceController controller = new ServiceController(ServiceName))
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        Logger.LogInfo($"Stopping the {ServiceName} service");
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Could not stop the {ServiceName} service before removal\r\n{ex}");
            }
        }

        /// <summary>
        /// Writes an error to the Windows event log as a last-resort diagnostics channel. The
        /// setup verbs run as deferred MSI custom actions, where msiexec does not capture process
        /// output, so without this an install failure produces no actionable diagnostics beyond a
        /// generic MSI error. Never throws.
        /// </summary>
        private static void WriteSetupError(string message)
        {
            try
            {
                Logger.LogError(message);
            }
            catch
            {
            }

            try
            {
                // The Logger's own source may not be registered yet on a first install that fails
                // early. The "Application" source always exists, so the message is never lost.
                EventLog.WriteEntry("Application", message, EventLogEntryType.Error);
            }
            catch
            {
            }
        }
    }
}
