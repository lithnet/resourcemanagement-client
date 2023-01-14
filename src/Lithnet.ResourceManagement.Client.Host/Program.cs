using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    throw new InvalidOperationException("Only a single arugment is allowed");
                }

                var pipeHost = new PipeHost();
                await pipeHost.OpenPipeAsync(args[0]);
                return 0;
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.ToString());
                return 1;
            }
        }
    }
}
