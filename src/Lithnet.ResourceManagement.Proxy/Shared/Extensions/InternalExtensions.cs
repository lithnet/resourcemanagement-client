using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    internal static class InternalExtensions
    {
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


        public async static Task<T> InvokeAsync<T, T1>(this ClientBase<T1> client, Func<T1, Task<T>> action) where T1 : class
        {
            T1 c = client.ChannelFactory.CreateChannel();

            try
            {
                ((IClientChannel)c).Open();
                T returnValue = await action(c);
                ((IClientChannel)c).Close();
                return returnValue;
            }
            catch
            {
                ((IClientChannel)c).Abort();
                throw;
            }
        }
    }
}
