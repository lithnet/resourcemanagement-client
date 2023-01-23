using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Lithnet.ResourceManagement.Client.Hosts
{
    internal class ComPipeHost : IPipeHost
    {
        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsUserAnAdmin();

        private IComHost comHost;

        public void OpenPipe(string pipeName)
        {
            comHost = TryCreateComComponent();
            comHost.OpenPipe(pipeName);
        }

        private IComHost TryCreateComComponent()
        {
            IComHost comhost;

            try
            {
                comhost = (IComHost)new ComHost();
            }
            catch (Exception ex) when (ex is FileNotFoundException fex || (ex is COMException cex && cex.HResult == -2147221164))
            {
                var expectedPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "fxhost\\Lithnet.ResourceManagement.Client.Host.exe");
                if (!File.Exists(expectedPath))
                {
                    expectedPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lithnet.ResourceManagement.Client.Host.exe");
                    if (!File.Exists(expectedPath))
                    {
                        throw new FileNotFoundException(expectedPath);
                    }
                }
                string[] runtimeAssemblies = Directory.GetFiles(Environment.ExpandEnvironmentVariables(@"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319"), "*.dll");

                var paths = new List<string>(runtimeAssemblies);
                paths.AddRange(Directory.GetFiles(Path.GetDirectoryName(expectedPath), "*.dll"));

                PathAssemblyResolver resolver = new PathAssemblyResolver(paths);
                using (var x = new System.Reflection.MetadataLoadContext(resolver))
                {
                    var assy = x.LoadFromAssemblyPath(expectedPath);
                    var an = assy.GetName();
                    var version = an.Version;
                    var fullName = an.FullName;
                    var codeBase = new Uri(assy.Location).AbsoluteUri;

                    this.RegisterCOMComponent("{dbdea27a-3d96-483c-97d1-9047c70141ac}", "Lithnet.ResourceManagement.Client.ComHost", "Lithnet.ResourceManagement.Client.Host.ComHost", fullName, codeBase, version.ToString());
                }

                comhost = (IComHost)new ComHost();
            }

            return comhost;
        }

        private void RegisterCOMComponent(string clsid, string prodId, string className, string assembly, string codeBase, string version)
        {
            RegistryKey root = IsUserAnAdmin() ? Registry.ClassesRoot : Registry.CurrentUser.OpenSubKey(@"Software\Classes", true);

            Console.WriteLine($"Registering COM component in {root.Name}");

            var classRootKey = root.CreateSubKey(prodId, true);
            classRootKey.SetValue(null, className);

            var classClsidKey = classRootKey.CreateSubKey("CLSID", true);
            classClsidKey.SetValue(null, clsid);

            var clssesclisidkey = root.OpenSubKey("CLSID", true);
            var classIdRoot = clssesclisidkey.CreateSubKey(clsid, true);
            classIdRoot.SetValue(null, className);

            var inprockey = classIdRoot.CreateSubKey("InprocServer32");
            inprockey.SetValue(null, "mscoree.dll");
            inprockey.SetValue("ThreadingModel", "Both");
            inprockey.SetValue("Class", className);
            inprockey.SetValue("Assembly", assembly);
            inprockey.SetValue("RuntimeVersion", "v4.0.30319");
            inprockey.SetValue("CodeBase", codeBase);

            inprockey = inprockey.CreateSubKey(version);
            inprockey.SetValue("Class", className);
            inprockey.SetValue("Assembly", assembly);
            inprockey.SetValue("RuntimeVersion", "v4.0.30319");
            inprockey.SetValue("CodeBase", codeBase);

            var progIdKey = classIdRoot.CreateSubKey("ProgId");
            progIdKey.SetValue(null, prodId);

            classIdRoot.CreateSubKey("Implemented Categories\\{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}");
        }

    }
}
