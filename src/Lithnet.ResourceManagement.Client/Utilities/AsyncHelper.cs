using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    internal static class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new
          TaskFactory(CancellationToken.None,
                      TaskCreationOptions.None,
                      TaskContinuationOptions.None,
                      TaskScheduler.Default);

        public static TResult Run<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper._myTaskFactory
              .StartNew<Task<TResult>>(func)
              .Unwrap<TResult>()
              .ConfigureAwait(false)
              .GetAwaiter()
              .GetResult();
        }

        public static void Run(Func<Task> func)
        {
            AsyncHelper._myTaskFactory
              .StartNew<Task>(func)
              .Unwrap()
              .ConfigureAwait(false)
              .GetAwaiter()
              .GetResult();
        }
    }
}
