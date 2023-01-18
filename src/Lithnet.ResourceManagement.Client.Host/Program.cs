using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal static class Program
    {
        private static void PreloadAssemblies()
        {
            var assemblies = new Dictionary<string, Assembly>();
            var executingAssembly = Assembly.GetExecutingAssembly();

            foreach (string resource in executingAssembly.GetManifestResourceNames().Where(n => n.EndsWith(".dll")))
            {
                using var stream = executingAssembly.GetManifestResourceStream(resource);
                if (stream == null)
                    continue;

                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                try
                {
                    Trace.WriteLine("Preloading assembly: " + resource);
                    assemblies.Add(resource, Assembly.Load(bytes));
                }
                catch (Exception ex)
                {
                    Trace.TraceError(string.Format("Failed to load: {0}\r\n{1}", resource, ex.ToString()));
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
            {
                var assemblyName = new AssemblyName(e.Name);

                var path = string.Format("{0}.dll", assemblyName.Name);

                if (assemblies.ContainsKey(path))
                {
                    return assemblies[path];
                }

                return null;
            };
        }

        public static async Task<int> Main(string[] args)
        {
            try
            {
                PreloadAssemblies();
                Logger.Initialize();

                if (args.Length != 1)
                {
                    Console.WriteLine("Incorrect command line arguments");
                    return 2;
                }

                var pipeHost = new PipeHost();
                await pipeHost.OpenPipeAsync(args[0]);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred and the host process is terminating");
                await Console.Error.WriteLineAsync(ex.ToString());
                return 1;
            }
        }
    }
}
