using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal class PipeHost
    {
        private static readonly ConcurrentDictionary<string, ServerInstance> servers = new ConcurrentDictionary<string, ServerInstance>();

        public void OpenPipe(string pipeName)
        {
            AsyncHelper.RunSync(async () => await this.OpenPipeAsync(pipeName));
        }

        public async Task OpenPipeAsync(string pipeName)
        {
            try
            {
                if (servers.ContainsKey(pipeName))
                {
                    throw new Exception("The specified pipe name is already in use");
                }

                var instance = new ServerInstance
                {
                    CancellationTokenSource = new CancellationTokenSource(),
                    PipeName = pipeName,
                };

                Logger.LogTrace($"Creating new pipe instance: {pipeName}");

                PipeRpcServer rpc = new PipeRpcServer(pipeName);
                Logger.LogTrace($"RPC server constructed for pipe {pipeName}");
                instance.Server = rpc;
                instance.ServerTask = rpc.StartNamedPipeServerAsync(instance.CancellationTokenSource.Token);
                Logger.LogTrace($"RPC server listening on pipe {pipeName}");

                servers.TryAdd(pipeName, instance);

                await instance.ServerTask;

                Logger.LogTrace($"RPC server has completed and pipe {pipeName} will be closed");
            }
            catch (OperationCanceledException) { }
        }

        public void ClosePipe(string pipeName)
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
