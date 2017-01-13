namespace Lithnet.ResourceManagement.Client
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Defines extension methods used in the application
    /// </summary>
    internal static class InternalExtensions
    {
        private const string FilterTextFormat = "<Filter xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" Dialect=\"http://schemas.microsoft.com/2006/11/XPathFilterDialect\" xmlns=\"http://schemas.xmlsoap.org/ws/2004/09/enumeration\">{0}</Filter>";

        public static string ToResourceManagementFilterXml(this string filter)
        {
            return string.Format(InternalExtensions.FilterTextFormat, System.Security.SecurityElement.Escape(filter));
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToCommaSeparatedString(this IEnumerable<string> strings)
        {
            return strings == null ? null : string.Join(", ", strings);
        }

        /// <summary>
        /// Disables the context manager for the specified client
        /// </summary>
        /// <typeparam name="T">The type of client proxy</typeparam>
        /// <param name="client">The client proxy to disable the context manager for</param>
        public static void DisableContextManager<T>(this ClientBase<T> client) where T : class
        {
            IContextManager property = client.ChannelFactory.GetProperty<IContextManager>();
            if (property != null)
            {
                property.Enabled = false;
            }
        }

        public static T Invoke<T, T1>(this ClientBase<T1> client, Func<T1, T> action) where T1 : class
        {

            T1 c = client.ChannelFactory.CreateChannel();

            try
            {
                ((IClientChannel)c).Open();
                T returnValue = action(c);
                ((IClientChannel)c).Close();
                return returnValue;
            }
            catch
            {
                ((IClientChannel)c).Abort();
                throw;
            }
        }

        public static List<string> ToList(this Enum p)
        {
            List<string> permissions = new List<string>();

            foreach (Enum value in Enum.GetValues(p.GetType()))
            {
                if (p.HasFlag(value) && Convert.ToInt32(value) != 0)
                {
                    permissions.Add(value.ToString());
                }
            }

            return permissions;
        }
    }
}
