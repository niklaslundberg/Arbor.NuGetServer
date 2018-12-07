using System;

namespace Arbor.NuGetServer.Api.Areas.CommonExtensions
{
    public static class SafeDisposer
    {
        public static void SafeDispose(this IDisposable disposable)
        {
            SafeDispose(disposable, null);
        }

        public static void SafeDispose(this IDisposable disposable, Action<Exception> exceptionHandler)
        {
            if (disposable is null)
            {
                return;
            }

            try
            {
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                exceptionHandler?.Invoke(ex);
            }
        }
    }
}
