using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Lithnet.ResourceManagement.Client.Host
{
    public static class ModuleInitializer
    {
        private static readonly string basePath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        private static string EventSourceName = "RMC";

        public static void Initialize()
        {
            try
            {
                try
                {
                    EventLog.WriteEntry(EventSourceName, $"RMC Init", EventLogEntryType.Error, 99);
                }
                catch { }

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception ex)
            {
                try
                {
                    EventLog.WriteEntry(EventSourceName, $"Could not initialize hosting module\r\n{ex}", EventLogEntryType.Error);
                }
                catch { }

                throw;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                EventLog.WriteEntry(EventSourceName, $"Unhandled exception in rmc host\r\n{(Exception)e.ExceptionObject}", EventLogEntryType.Error, 99);
            }
            catch { }
        }

        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Trace.WriteLine($"Attempting to resolve assembly {args.Name}");
            EventLog.WriteEntry(EventSourceName, $"Trying to solve assemly {args.Name}", EventLogEntryType.Warning, 99);
            var name = new AssemblyName(args.Name);
            //if (name.Name == "System.Runtime.CompilerServices.Unsafe")
            //{
            //    return typeof(System.Runtime.CompilerServices.Unsafe).Assembly;
            //}

            string assyPath = Path.Combine(basePath, $"{name.Name}.dll");

            Trace.WriteLine($"Looking for a file named {assyPath}");
            if (File.Exists(assyPath))
            {
                EventLog.WriteEntry(EventSourceName, $"Resolved assemly {assyPath}", EventLogEntryType.Information, 99);

                return Assembly.LoadFrom(assyPath);
            }

            Trace.WriteLine($"File not found {assyPath}");

            return null;
        }
    }
}