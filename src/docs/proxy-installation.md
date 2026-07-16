# Proxy installation guide

The **Lithnet Resource Management Client Proxy** installer delivers the proxy executable used by the Lithnet Resource Management Client. The same installer serves two distinct roles, and which one you need depends on where you run it:

| Role | Where you install it | Why |
| ---- | -------------------- | --- |
| **Remote proxy service** | On the FIM/MIM server | Hosts the `LithnetRMCProxy` Windows service that `RemoteProxy` (`rmc://`) connections use. This is what lets clients on Linux, macOS, and .NET (Core) perform every operation, including approvals. |
| **Local proxy host** | On client machines | Places a permanent, trusted copy of the proxy executable in Program Files for `LocalProxy` connections. Required on machines where an application control policy (WDAC / AppLocker) blocks the client library from extracting its embedded copy to `%TEMP%`. |

Both roles come from the one MSI; the installer adapts to the machine it runs on.

## Requirements

* Windows Server or Windows client, 64-bit
* .NET Framework 4.7.2 or later
* Administrative rights to install
* To host the **remote proxy service**: the FIM/MIM Service must be installed on the same machine

## Installing on the FIM/MIM server

1. Download the installer from the [releases page](https://github.com/lithnet/resourcemanagement-client/releases).
2. Run the installer. When it detects the FIM/MIM Service on the machine, the summary page offers an **Install the proxy service** option, enabled by default.
3. Complete the installation.

When the service option is selected, the installer:

* Registers a Windows service named `LithnetRMCProxy` (display name *Lithnet Resource Management Proxy*), set to start automatically, with a dependency on the FIM/MIM Service. The service runs as `NT AUTHORITY\NetworkService` and needs no credentials of its own -- it impersonates each connecting user when calling the MIM service.
* Creates a local group named **Lithnet RMC Proxy Users**, seeded with the local Administrators group, and configures the service to only accept connections from its members.
* Registers the event log sources the service writes to, and starts the service. It listens on TCP port **5735**.

After installation, two manual steps remain:

1. **Authorize your users**: add the accounts or groups that will connect to the **Lithnet RMC Proxy Users** local group.
2. **Open the firewall**: allow inbound TCP 5735. The installer does not create a firewall rule.

## Installing on client machines

Run the same installer on the client machine. When the FIM/MIM Service is not present, the service option is unavailable and the installer simply places the proxy executable in Program Files and registers its location in the registry (`HKLM\SOFTWARE\Lithnet\Resource Management Client\HostPath`). Every client on the machine automatically finds and prefers the installed copy over extracting its embedded one -- no client configuration is needed.

This matters on machines with an application control policy: the embedded copy extracts to `%TEMP%`, which such policies routinely block, while the installed copy lives in Program Files where it can be trusted by publisher or path rule. The executable is Authenticode-signed by Lithnet.

## Silent installation

```
msiexec /i Lithnet.ResourceManagement.Proxy.Setup.msi /qn
```

The proxy service is installed by default whenever the FIM/MIM Service is present on the machine. To install without the service (files and registry marker only), pass `INSTALL_SERVICE=0`:

```
msiexec /i Lithnet.ResourceManagement.Proxy.Setup.msi /qn INSTALL_SERVICE=0
```

On machines without the FIM/MIM Service, the service is never installed, regardless of this property.

## Upgrades

Upgrading installs the new version in place. The service is stopped during the upgrade and restarted afterwards, and the **Lithnet RMC Proxy Users** group, its membership, and the service's registry configuration are all preserved.

## Uninstalling

Uninstalling stops and removes the `LithnetRMCProxy` service, along with its registry configuration under the service key. The **Lithnet RMC Proxy Users** group and its membership are deliberately left in place, so a later reinstall restores service without having to re-authorize every user. Delete the group manually if you no longer need it.

## Configuration reference

The service reads its settings from the registry at `HKLM\SYSTEM\CurrentControlSet\Services\LithnetRMCProxy` when it starts, so restart the service after making changes.

| Value | Type | Default | Purpose |
| ----- | ---- | ------- | ------- |
| `ProxyPort` | DWORD | `5735` | The TCP port the proxy listens on |
| `ResourceManagementServicePort` | DWORD | `5725` | The port of the local FIM/MIM Service |
| `AuthorizedUsers` | String | SID of *Lithnet RMC Proxy Users* | The SID of a single group whose members are authorized to connect. The value must be a **group** SID -- a user's own SID never matches the membership check. If the value is missing or invalid, the service falls back to the built-in Administrators group. |

## Logging and troubleshooting

The service writes to the **Application** event log:

* Source `LithnetRMCProxy` -- service start and stop events.
* Source `LithnetResourceManagementClientFxHost` -- operational logging, including a log entry for every denied connection that names the group it enforced and the identity it authenticated.

If the installer's service setup fails, the error is also written to the Application event log.

For client-side configuration and connection troubleshooting, see the [connection guide](https://github.com/lithnet/resourcemanagement-client/wiki/Connection-guide).
