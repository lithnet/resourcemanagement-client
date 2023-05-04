using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace Lithnet.ResourceManagement.Client
{
    internal class ExePipeHost : IPipeHost
    {
        private Process hostProcess;

        private static string embeddedHostHash;
        private static byte[] embeddedBinary;

        public string HostLocation { get; private set; }

        public void OpenPipe(string pipeName, ResourceManagementClientOptions p)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            var path = GetOrExtractHost(p);

            if (path == null)
            {
                throw new FileNotFoundException("Unable to locate framework host", "Lithnet.ResourceManagement.Proxy.exe");
            }

            this.HostLocation = path;

            Trace.WriteLine($"Attempting to invoke {path}");

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                Arguments = pipeName,
                CreateNoWindow = true,
                ErrorDialog = false,
                FileName = path,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(path),
                LoadUserProfile = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            this.hostProcess = Process.Start(startInfo);
            this.hostProcess.OutputDataReceived += this.Process_OutputDataReceived;
            this.hostProcess.ErrorDataReceived += this.Process_ErrorDataReceived;
            this.hostProcess.Exited += this.Process_Exited;
            Trace.WriteLine($"Created host process {this.hostProcess.Id}");

            if (this.hostProcess.HasExited)
            {
                throw new Exception("Host process exited");
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("The host process exited");
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"Error data received: {e.Data}");
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"Output data received: {e.Data}");
        }

        private static string GetOrExtractHost(ResourceManagementClientOptions p)
        {
            if (!string.IsNullOrWhiteSpace(p.RmcHostExe))
            {
                return p.RmcHostExe;
            }

            var installedPath = GetInstalledHostPath();
            if (!string.IsNullOrWhiteSpace(installedPath))
            {
                Trace.WriteLine("Found installed host");
                return installedPath;
            }

            var temp = Path.GetTempPath();
            var hostFile = Path.Combine(temp, "LithnetRmcProxy", "Lithnet.ResourceManagement.Proxy.exe");
            if (File.Exists(hostFile))
            {
                if (GetFileHash(hostFile) == GetEmbeddedBinaryHash())
                {
                    Trace.WriteLine("Found existing extracted binary");
                    return hostFile;
                }
            }

            Trace.WriteLine("Extracting embedded binary");
            Directory.CreateDirectory(Path.GetDirectoryName(hostFile));
            File.WriteAllBytes(hostFile, GetEmbeddedBinary());

            return hostFile;
        }

        public static string GetInstalledHostPath()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return null;
            }

            return Registry.LocalMachine.GetValue(@"Software\Lithnet\Resource Management Client\HostPath", null) as string;
        }

        public static string GetFileHash(string filePath)
        {
            using SHA256 SHA256 = SHA256.Create();
            using FileStream fileStream = File.OpenRead(filePath);
            return Convert.ToBase64String(SHA256.ComputeHash(fileStream));
        }

        private static string GetEmbeddedBinaryHash()
        {
            if (embeddedHostHash == null)
            {
                using SHA256 hasher = SHA256.Create();
                embeddedHostHash = Convert.ToBase64String(hasher.ComputeHash(GetEmbeddedBinary()));
            }

            return embeddedHostHash;
        }

        private static byte[] GetEmbeddedBinary()
        {
            if (embeddedBinary == null)
            {
                var assembly = typeof(ExePipeHost).Assembly;
                var resourceName = "Host.exe";
                var resource = assembly?.GetManifestResourceStream(resourceName);

                if (resource == null)
                {
                    throw new ResourceNotFoundException($"The embedded resource {resourceName} was not found");
                }

                using var ms = new MemoryStream();
                resource.CopyTo(ms);
                embeddedBinary = ms.ToArray();
            }

            return embeddedBinary;
        }
        private static string FindHostPath()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            List<string> probePaths = new List<string>()
            {
                RmcConfiguration.FxHostPath,
                Registry.CurrentUser.GetValue("Software\\Lithnet\\ResourceManagementClient\\FxHostPath", null) as string,
                Registry.LocalMachine.GetValue("Software\\Lithnet\\ResourceManagementClient\\FxHostPath", null) as string,
                GetParentPath(Assembly.GetExecutingAssembly()?.Location),
                GetParentPath(Assembly.GetCallingAssembly()?.Location),
                GetParentPath(Assembly.GetEntryAssembly()?.Location)
            };

            foreach (var probePath in probePaths)
            {
                if (string.IsNullOrWhiteSpace(probePath))
                {
                    continue;
                }

                Trace.WriteLine($"Looking in probe path {probePath}");

                var result = ProbePath(probePath);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static string GetParentPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return Path.GetDirectoryName(path);
        }

        private static string ProbePath(string path)
        {
            var expectedPath = Path.Combine(path, "fxhost\\Lithnet.ResourceManagement.Proxy.exe");

            if (File.Exists(expectedPath))
            {
                Trace.WriteLine($"Found file at {expectedPath}");
                return expectedPath;
            }

            expectedPath = Path.Combine(path, "Lithnet.ResourceManagement.Proxy.exe");
            if (File.Exists(expectedPath))
            {
                Trace.WriteLine($"Found file at {expectedPath}");
                return expectedPath;
            }

            Trace.WriteLine($"File was not found at {path}");

            return null;
        }

        public static bool HasHostExe()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            return FindHostPath() != null;
        }
    }
}
