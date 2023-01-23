using System;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                Logger.Initialize();

                if (args.Length != 1)
                {
                    Console.WriteLine("Incorrect command line arguments");
                    return 2;
                }

                if (args[0] == "/register")
                {
                    try
                    {
                        Logger.SetupEventSource();
                        Console.WriteLine("Event log successfully registered");
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to register event source");
                        Console.WriteLine(ex.ToString());
                        return 3;
                    }
                }
                else if (args[0] == "/unregister")
                {
                    try
                    {
                        Logger.DeleteEventSource();
                        Console.WriteLine("Event log successfully unregistered");
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to unregister event source");
                        Console.WriteLine(ex.ToString());
                        return 3;
                    }
                }
                else
                {
                    var pipeHost = new PipeHost();
                    await pipeHost.OpenPipeAsync(args[0]);
                    return 0;
                }
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
