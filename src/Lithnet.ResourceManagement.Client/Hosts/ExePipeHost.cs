using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Lithnet.ResourceManagement.Client.Hosts
{
    internal class ExePipeHost : IPipeHost
    {
        private Process hostProcess;

        public void OpenPipe(string pipeName)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            var path = FindHostPath();

            if (path == null)
            {
                throw new FileNotFoundException("Unable to locate framework host", "Lithnet.ResourceManagement.Client.Host.exe");
            }

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
            Console.WriteLine($"Output data recieved: {e.Data}");
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
            var expectedPath = Path.Combine(path, "fxhost\\Lithnet.ResourceManagement.Client.Host.exe");

            if (File.Exists(expectedPath))
            {
                Trace.WriteLine($"Found file at {expectedPath}");
                return expectedPath;
            }

            expectedPath = Path.Combine(path, "Lithnet.ResourceManagement.Client.Host.exe");
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
