using System.Collections;
using System.Linq;

namespace Lithnet.ResourceManagement.Client
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;

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

        public static bool HasOne<T>(this IEnumerable<T> enumerable)
        {
            bool hasOneItem = false;

            IList list = enumerable as IList;

            if (list != null)
            {
                return list.Count == 1;
            }

            foreach (T item in enumerable)
            {
                if (hasOneItem)
                {
                    return false;
                }

                hasOneItem = true;
            }

            return hasOneItem;
        }

        /// <summary>
        /// Iterates into an IEnumerable as few times as possible to determine if the collection contains 0, 1, or > 1 elements. Collections with more than one item will return a maximum value of 2
        /// </summary>
        /// <param name="enumerable">The collection to count</param>
        /// <returns>Either, zero, one, or two</returns>
        public static int CountZeroOneOrMore(this IEnumerable enumerable)
        {
            int count = 0;

            if (enumerable == null)
            {
                return 0;
            }

            IList list = enumerable as IList;

            if (list != null)
            {
                return list.Count >= 2 ? 2 : list.Count;
            }

            IEnumerator e = enumerable.GetEnumerator();

            while (e.MoveNext())
            {
                count++;

                if (count >= 2)
                {
                    return 2;
                }
            }

            return count;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static bool HasMoreThanOne<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Skip(1).Any();
        }

        public static bool HasOnlyOne<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Any() && !enumerable.Skip(1).Any();
        }

        public static T[] GetFirstTwoItems<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Take(2).ToArray();
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

        public static async Task<T> Invoke<T, T1>(this ClientBase<T1> client, Task<T> action) where T1 : class
        {
            T1 c = client.ChannelFactory.CreateChannel();

            try
            {
                ((IClientChannel)c).Open();
                T returnValue = await action;
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
