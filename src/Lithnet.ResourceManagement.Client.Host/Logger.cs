using System;
using System.Diagnostics;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal static class Logger
    {
        private const string SourceName = "LithnetResourceManagementClientFxHost";

        private static bool canLogEventLog;

        public static void Initialize()
        {
            SetupEventSource();
        }

        public static void SetupEventSource()
        {
            bool eventLogExists = false;

            try
            {
                eventLogExists = EventLog.SourceExists(SourceName);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unable to search for event source {0}\r\n{1}", SourceName, ex.ToString());
            }

            if (!eventLogExists)
            {
                try
                {
                    EventLog.CreateEventSource(SourceName, "Application");
                    canLogEventLog = true;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to register event source {0}\r\n{1}", SourceName, ex.ToString());
                    canLogEventLog = false;
                }
            }
            else
            {
                canLogEventLog = true;
            }
        }

        public static void DeleteEventSource()
        {
            bool eventLogExists = false;

            try
            {
                eventLogExists = EventLog.SourceExists(SourceName);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unable to search for event source {0}\r\n{1}", SourceName, ex.ToString());
            }

            if (eventLogExists)
            {
                try
                {
                    EventLog.DeleteEventSource(SourceName);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to delete event source {0}\r\n{1}", SourceName, ex.ToString());
                }
            }
        }

        public static void LogError(string message, params object[] args)
        {
            if (canLogEventLog)
            {
                try
                {
                    EventLog.WriteEntry(SourceName, string.Format(message, args), EventLogEntryType.Error);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to log event\r\n{1}", ex.ToString());
                }
            }

            Trace.TraceError(message, args);
        }

        public static void LogError(Exception ex, string message, params object[] args)
        {
            LogError($"{message}\r\n{ex}", args);
        }

        public static void LogInfo(string message, params object[] args)
        {
            if (canLogEventLog)
            {
                try
                {
                    EventLog.WriteEntry(SourceName, string.Format(message, args), EventLogEntryType.Information);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to log event\r\n{1}", ex.ToString());
                }
            }

            Trace.TraceInformation(message, args);
        }

        public static void LogTrace(string message, params object[] args)
        {
            Trace.TraceInformation(string.Format(message, args));
        }

        public static void LogWarning(string message, params object[] args)
        {
            if (canLogEventLog)
            {
                try
                {
                    EventLog.WriteEntry(SourceName, string.Format(message, args), EventLogEntryType.Warning);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to log event\r\n{1}", ex.ToString());
                }
            }

            Trace.TraceWarning(message, args);
        }
    }
}
