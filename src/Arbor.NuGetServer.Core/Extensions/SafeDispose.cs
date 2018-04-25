using System;
using System.Diagnostics;

namespace Arbor.NuGetServer.Core.Extensions
{
    public static class SafeDispose
    {
        public static void Dispose(IDisposable disposable, Action<Exception> exceptionHandler = null)
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
