using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
//using RGiesecke.DllExport;

namespace Lithnet.ResourceManagement.Client.Host
{
    public static class DllExports
    {
        private static ConcurrentDictionary<string, ServerInstance> servers = new ConcurrentDictionary<string, ServerInstance>();

        //[DllExport]
        public static void OpenPipe(string pipeName)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ModuleInitializer.CurrentDomain_AssemblyResolve;

            try
            {
                if (servers.ContainsKey(pipeName))
                {
                    Trace.WriteLine($"Pipe {pipeName} already exists");
                    throw new Exception("The specified pipe name is already in use");
                }

                var instance = new ServerInstance
                {
                    CancellationTokenSource = new CancellationTokenSource(),
                    PipeName = pipeName,
                };

                Trace.WriteLine($"Creating new pipe instance: {pipeName}");

                RpcServer rpc = new RpcServer(pipeName);
                Trace.WriteLine($"RPC server constructed for pipe {pipeName}");
                instance.Server = rpc;
                instance.ServerTask = rpc.StartNamedPipeServerAsync(instance.CancellationTokenSource.Token);
                Trace.WriteLine($"RPC server listening on pipe {pipeName}");

                servers.TryAdd(pipeName, instance);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("RMC", ex.ToString());
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        //[DllExport]
        public static void ClosePipe(string pipeName)
        {
            if (servers.ContainsKey(pipeName))
            {
                var instance = servers[pipeName];
                instance.CancellationTokenSource.Cancel();
                instance.ServerTask = null;
                instance.Server = null;
                servers.TryRemove(pipeName, out _);
            }
        }
    }
}
