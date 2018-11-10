using System;
using System.Diagnostics;

namespace Arbor.NuGetServer.Api.Areas.CommonExtensions
{
    public static class SafeDisposer
    {
        public static void SafeDispose(this IDisposable disposable, Action<Exception> exceptionHandler = null)
        {
            if (disposable is null)
            {
                return;
            }

            try
            {
                Debug.WriteLine($"Disposing {disposable}");
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                exceptionHandler?.Invoke(ex);
            }
        }
    }
}
