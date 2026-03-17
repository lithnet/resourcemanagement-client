# Connection Guide - Lithnet Resource Management Client v2 with .NET support

## Introduction

Version 2 of the Lithnet Resource Management Client introduces cross-platform support for connecting to the Microsoft FIM/MIM Service from .NET Core and modern .NET applications. In v1, the library relied exclusively on WCF's `WSHttpBinding` to communicate with the FIM Service, which is only available on .NET Framework. In v2, the library targets .NET Standard 2.0 and provides four distinct connection modes to accommodate the various runtime, platform, and deployment constraints you may encounter.

This document explains each connection mode, how the library selects one, the trade-offs involved, and how to configure each option.

---

## Connection modes at a glance

| Mode | Enum value | URL scheme | .NET Framework | .NET Core (Windows) | .NET Core (Linux / macOS) | Approval support | Server-side changes |
|---|---|---|---|---|---|---|---|
| **Direct WS-Http** | `DirectWsHttp` | `http://` | ✅ | ❌ | ❌ | ✅ | None |
| **Direct Net.Tcp** | `DirectNetTcp` | `net.tcp://` | ✅ | ✅ | ✅ | ❌ | Net.Tcp bindings required |
| **Local Proxy** | `LocalProxy` | `pipe://` | N/A | ✅ (Windows only) | ❌ | ✅ | None |
| **Remote Proxy** | `RemoteProxy` | `rmc://` | ✅ | ✅ (.NET 8+) | ✅ (.NET 8+) | ✅ | Proxy service required |

---

## Automatic mode selection

When `ConnectionMode` is set to `Auto` (the default), the library inspects the `BaseUri` scheme and the current runtime to choose the best available transport. The selection logic proceeds as follows:

1. **URL scheme detection** -- If the `BaseUri` has an explicit scheme, that scheme determines the mode:
   - `net.tcp://` → `DirectNetTcp`
   - `rmc://` → `RemoteProxy`
   - `pipe://` → `LocalProxy`
   - `http://` or a plain hostname → continues to step 2

2. **Runtime detection** (when the scheme is `http://` or unspecified):
   - If running on **.NET Framework**, `DirectWsHttp` is used.
   - If running on **.NET Core on Windows** and the embedded proxy host executable is available, `LocalProxy` is used.
   - Otherwise, `RemoteProxy` is used as a fallback.

You can override automatic selection by setting the `ConnectionMode` property on `ResourceManagementClientOptions` to a specific value.

> **Note:** If you explicitly request `DirectWsHttp` from a .NET Core process, the library will ignore the request (because WCF's `WSHttpBinding` is not available) and fall through to automatic detection.

---

## Connection mode details

### 1. Direct WS-Http (`DirectWsHttp`)

This is the traditional connection mode used by v1 of the library. It uses WCF's `WSHttpBinding` to communicate directly with the FIM/MIM Service over HTTP on port **5725**.

**When to use:**
- Your application targets **.NET Framework**.
- You are running on Windows and the FIM/MIM Service is reachable over the network.

**Advantages:**
- No additional components or server-side configuration needed.
- Full feature support, including **approvals**.
- Out-of-box support -- this is the same transport FIM/MIM has always used.

**Limitations:**
- Only works on .NET Framework. WCF's `WSHttpBinding` is not available on .NET Core.

**Configuration:**

```csharp
var client = new ResourceManagementClient(new ResourceManagementClientOptions
{
    BaseUri = "http://mimserver:5725",
    ConnectionMode = ConnectionMode.DirectWsHttp
});
```

Or, when running on .NET Framework, simply provide a hostname and the library will default to this mode:

```csharp
var client = new ResourceManagementClient("mimserver");
```

**.NET Framework app.config:**

```xml
<configuration>
  <configSections>
    <section name="lithnetResourceManagementClient"
             type="Lithnet.ResourceManagement.Client.ClientConfigurationSection, Lithnet.ResourceManagement.Client"/>
  </configSections>

  <lithnetResourceManagementClient
    resourceManagementServiceBaseAddress="http://mimserver:5725" />
</configuration>
```

---

### 2. Direct Net.Tcp (`DirectNetTcp`)

This mode connects to the FIM/MIM Service over `net.tcp` bindings. The FIM Service does not expose Net.Tcp endpoints by default, so you must manually add them to the FIM Service configuration on the server.

**When to use:**
- You need to connect from **.NET Core** (Windows, Linux, or macOS) and cannot or do not want to deploy the remote proxy service.
- You have administrative access to the FIM/MIM server to add Net.Tcp bindings.
- You do **not** need approval endpoint support.

**Advantages:**
- Works on all platforms and all .NET versions (Framework and Core).
- No additional proxy component needed -- direct communication with the FIM Service.

**Limitations:**
- **Approvals are not supported.** Attempting to use the approval API will throw a `NotImplementedException`.
- Requires manual modification of the FIM Service's `app.config` to add Net.Tcp endpoints and enable the Net.Tcp WAS adapter on the server.

**Default port:** `5736`

**Server-side setup:**

You must add Net.Tcp bindings to the FIM Service's `Microsoft.ResourceManagement.Service.exe.config` file. The service endpoints for `Resource`, `ResourceFactory`, and `Enumeration` need corresponding `net.tcp` endpoints. Refer to the [Microsoft documentation on WCF net.tcp bindings](https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/net-tcp-port-sharing) for general guidance on configuring Net.Tcp. You will also need to ensure that the `Net.Tcp Port Sharing Service` Windows service is running on the FIM server.

**Configuration:**

```csharp
var client = new ResourceManagementClient(new ResourceManagementClientOptions
{
    BaseUri = "net.tcp://mimserver:5736",
    ConnectionMode = ConnectionMode.DirectNetTcp
});
```

Or simply use a `net.tcp://` URI and the library will automatically select this mode:

```csharp
var client = new ResourceManagementClient(new ResourceManagementClientOptions
{
    BaseUri = "net.tcp://mimserver"
});
```

If you omit the port, or specify port `5725` or `80`, the library will automatically substitute the default Net.Tcp port of **5736**.

---

### 3. Local Proxy (`LocalProxy`)

This mode is designed for **.NET Core applications running on Windows**. The library automatically extracts and launches a small embedded .NET Framework proxy executable (`Lithnet.ResourceManagement.Proxy.exe`) as a child process. The .NET Core process communicates with this proxy over **named pipes**, and the proxy in turn makes the native WS-Http WCF calls to the FIM Service on behalf of the caller.

**When to use:**
- Your application targets **.NET Core** and runs on **Windows**.
- You want the simplest setup path that doesn't require any server-side changes.
- You need **approval** support.

**Advantages:**
- No server-side changes required.
- Full feature support, including **approvals**.
- The proxy executable is embedded in the NuGet package and is automatically extracted and launched -- no manual installation is necessary.
- Runs with the identity of the calling process.

**Limitations:**
- **Windows only.** Named pipes and the .NET Framework host are Windows-specific.
- Slightly higher overhead than direct connections due to the inter-process JSON-RPC communication over named pipes.

**How it works:**

1. The library extracts `Lithnet.ResourceManagement.Proxy.exe` from its embedded resources to a temporary directory (or locates it via the registry or a configured path).
2. It launches the proxy as a child process, passing a unique pipe name as a command-line argument.
3. The proxy opens a `NamedPipeServerStream` and waits for the client.
4. The .NET Core process connects over the named pipe and uses [StreamJsonRpc](https://github.com/microsoft/vs-streamjsonrpc) to send requests.
5. The proxy translates the JSON-RPC calls into native WCF WS-Http calls to `http://localhost:5725` (or the configured FIM Service address).

**Configuration:**

The library will automatically select this mode on Windows when the host executable is available. You can also force it:

```csharp
var client = new ResourceManagementClient(new ResourceManagementClientOptions
{
    BaseUri = "http://mimserver:5725",
    ConnectionMode = ConnectionMode.LocalProxy
});
```

Or use the `pipe://` scheme for automatic selection:

```csharp
var client = new ResourceManagementClient(new ResourceManagementClientOptions
{
    BaseUri = "pipe://mimserver"
});
```

**Custom host executable path:**

If you have installed the proxy or placed it in a custom location, you can specify the path:

```csharp
var options = new ResourceManagementClientOptions
{
    BaseUri = "http://mimserver:5725",
    ConnectionMode = ConnectionMode.LocalProxy,
    RmcHostExe = @"C:\Program Files\Lithnet\Lithnet.ResourceManagement.Proxy.exe"
};
```

The library also checks the following locations for the proxy executable, in order:
1. The `RmcHostExe` property on `ResourceManagementClientOptions`.
2. The `RmcConfiguration.FxHostPath` static property.
3. The Windows registry key `HKCU\Software\Lithnet\ResourceManagementClient\FxHostPath`.
4. The Windows registry key `HKLM\Software\Lithnet\ResourceManagementClient\FxHostPath`.
5. The directory of the executing, calling, and entry assemblies (looking for `fxhost\Lithnet.ResourceManagement.Proxy.exe` or `Lithnet.ResourceManagement.Proxy.exe`).
6. The embedded resource, extracted to the `%TEMP%\LithnetRmcProxy\` directory.

---

### 4. Remote Proxy (`RemoteProxy`)

This mode connects to the **Lithnet Resource Management Proxy** service, which is installed as a Windows service on the FIM/MIM server. The client communicates with the proxy over a TCP connection secured with Negotiate (SPNEGO/Kerberos) authentication. The proxy receives JSON-RPC requests, translates them into WS-Http WCF calls to `http://127.0.0.1:5725` on the local machine, and returns the results.

**When to use:**
- Your application targets **.NET Core** and runs on **any operating system** (Windows, Linux, macOS).
- You want full feature support, including **approvals**.
- You can install the proxy service on the FIM/MIM server (or a machine that can reach it on `localhost`).

**Advantages:**
- Cross-platform -- works on Windows, Linux, and macOS.
- Full feature support, including **approvals**.
- Secure -- communication is authenticated and encrypted using Negotiate stream authentication.
- The proxy handles all WCF complexity server-side.

**Limitations:**
- Requires installation and configuration of the **Lithnet Resource Management Proxy** Windows service on the FIM/MIM server.
- On non-Windows platforms, requires **.NET 8 or later** (due to `NegotiateStream` cross-platform support).
- The connecting user must be a member of the authorized proxy users group (defaults to the local `Administrators` group on the proxy server, configurable via registry).

**Default port:** `5735`

**Server-side setup:**

1. Install the `Lithnet.ResourceManagement.Proxy` service on the FIM/MIM server. The installer registers a Windows service named `LithnetRMCProxy`.
2. Start the service. It listens on TCP port **5735** by default.
3. (Optional) Configure the authorized users group and ports via the registry at `HKLM\SYSTEM\CurrentControlSet\Services\LithnetRMCProxy`:
   - `ProxyPort` (DWORD) -- the TCP port the proxy listens on (default: `5735`).
   - `ResourceManagementServicePort` (DWORD) -- the port of the local FIM Service (default: `5725`).
   - `AuthorizedUsers` (String) -- the SID of a group whose members are authorized to connect (default: the built-in Administrators group).

**Configuration:**

```csharp
var client = new ResourceManagementClient(new ResourceManagementClientOptions
{
    BaseUri = "rmc://mimserver:5735",
    ConnectionMode = ConnectionMode.RemoteProxy
});
```

Or simply use the `rmc://` scheme for automatic selection:

```csharp
var client = new ResourceManagementClient(new ResourceManagementClientOptions
{
    BaseUri = "rmc://mimserver"
});
```

If you omit the port, or specify port `5725` or `80`, the library will automatically substitute the default proxy port of **5735**.

**Authentication:**

The remote proxy uses `NegotiateStream` for mutual authentication. The client authenticates with the proxy using:
- The current Windows identity (default), or
- Explicit credentials provided via the `Username` and `Password` properties.

The SPN used for authentication defaults to `FIMService/{hostname}` and can be overridden with the `Spn` property.

```csharp
var client = new ResourceManagementClient(new ResourceManagementClientOptions
{
    BaseUri = "rmc://mimserver",
    Username = @"DOMAIN\serviceaccount",
    Password = "password",
    Spn = "FIMService/mimserver.contoso.com"
});
```

---

## Choosing the right connection mode

Use the following decision tree to select the connection mode that best fits your scenario:

```
Is your application running on .NET Framework?
├── Yes → Use DirectWsHttp (default, no changes needed)
└── No (running on .NET Core / .NET 5+)
    │
    ├── Do you need approval support?
    │   ├── No → Use DirectNetTcp (add net.tcp bindings to FIM server)
    │   └── Yes
    │       │
    │       ├── Are you running on Windows?
    │       │   ├── Yes → Use LocalProxy (automatic, no server changes)
    │       │   └── No → Use RemoteProxy (install proxy service on FIM server)
    │       │
    │       └── Can you install the proxy on the FIM server?
    │           ├── Yes → Use RemoteProxy (best cross-platform option)
    │           └── No → Use DirectNetTcp (but approvals will not be available)
    │
    └── Do you want to avoid any server-side changes?
        ├── Yes, and on Windows → Use LocalProxy
        └── Otherwise → Use RemoteProxy or DirectNetTcp
```

### Summary of trade-offs

| Consideration | DirectWsHttp | DirectNetTcp | LocalProxy | RemoteProxy |
|---|---|---|---|---|
| **Platform support** | .NET Framework only | All | .NET Core on Windows | All (.NET 8+ on non-Windows) |
| **Approvals** | ✅ | ❌ | ✅ | ✅ |
| **Server-side changes** | None | Net.Tcp bindings | None | Proxy service install |
| **Performance** | Native WCF | Native WCF | IPC overhead (named pipes) | Network + IPC overhead |
| **Complexity** | Low | Medium | Low | Medium |

---

## Configuration reference

### `ResourceManagementClientOptions` properties

| Property | Type | Default | Description |
|---|---|---|---|
| `BaseUri` | `string` | `http://localhost:5725` | The URI or hostname of the MIM Service. The scheme determines the connection mode when `ConnectionMode` is `Auto`. |
| `ConnectionMode` | `ConnectionMode` | `Auto` | The connection mode to use. Set to a specific value to override automatic detection. |
| `Username` | `string` | `null` | The username to authenticate with. Leave blank to use the current Windows identity. |
| `Password` | `string` | `null` | The password for the specified username. |
| `Spn` | `string` | `null` | The service principal name. Defaults to `FIMService/{hostname}` if not specified. |
| `ConcurrentConnectionLimit` | `int` | `10000` | The maximum number of concurrent connections to the MIM Service. |
| `ConnectTimeoutSeconds` | `int` | `30` | The maximum time in seconds to wait for a connection to be established. |
| `RecieveTimeoutSeconds` | `int` | `1200` | The maximum time in seconds to wait for incoming data. |
| `SendTimeoutSeconds` | `int` | `1200` | The maximum time in seconds to wait for outgoing data. |
| `RmcHostExe` | `string` | `null` | The path to the local proxy host executable. Only used with `LocalProxy` mode. |

### URL scheme reference

| Scheme | Example | Resulting mode | Default port |
|---|---|---|---|
| `http://` | `http://mimserver:5725` | `DirectWsHttp` (.NET Fx) or auto-detect (.NET Core) | 5725 |
| `net.tcp://` | `net.tcp://mimserver:5736` | `DirectNetTcp` | 5736 |
| `rmc://` | `rmc://mimserver:5735` | `RemoteProxy` | 5735 |
| `pipe://` | `pipe://mimserver` | `LocalProxy` | 5725 |
| *(hostname only)* | `mimserver` | Same as `http://` | 5725 |

---

## Troubleshooting

### `PlatformNotSupportedException` when using `LocalProxy`
The local proxy mode is only available on Windows. If you are running on Linux or macOS, use `DirectNetTcp` or `RemoteProxy` instead.

### `PlatformNotSupportedException: Using 'RemoteProxy' requires .NET 8 or later`
On non-Windows platforms, the `NegotiateStream` authentication used by the remote proxy requires .NET 8 or later. Upgrade your target framework, or use `DirectNetTcp` if approvals are not required.

### `NotImplementedException` when calling approval APIs with `DirectNetTcp`
The Net.Tcp connection mode does not support the approval endpoint. Switch to `DirectWsHttp` (on .NET Framework), `LocalProxy` (on .NET Core Windows), or `RemoteProxy` for approval support.

### `RmcProxyConnectionException: Could not connect to the RMC proxy`
Ensure the Lithnet Resource Management Proxy service is installed and running on the target server, and that TCP port **5735** (or your configured port) is accessible from the client machine.

### `UnauthorizedAccessException: Access to the RMC proxy was denied`
The connecting user must be a member of the authorized proxy users group on the proxy server. By default, this is the local `Administrators` group. The authorized group can be changed via the `AuthorizedUsers` registry value under `HKLM\SYSTEM\CurrentControlSet\Services\LithnetRMCProxy`.

### The local proxy executable cannot be found
The library looks for `Lithnet.ResourceManagement.Proxy.exe` in several locations. You can set the path explicitly via the `RmcHostExe` option, or set the `RmcConfiguration.FxHostPath` static property before creating the client. The library will also attempt to extract an embedded copy to `%TEMP%\LithnetRmcProxy\`.
