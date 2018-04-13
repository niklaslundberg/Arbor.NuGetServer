using System;
using System.Security.Cryptography;

namespace Arbor.NuGetServer.Core.Extensions
{
    public static class SafeDispose
    {
        public static void Dispose(IDisposable disposable, Action<Exception> exceptionHandler = null)
        {
            try
            {
                disposable?.Dispose();
            }
            catch (Exception ex)
            {
                exceptionHandler?.Invoke(ex);
            }
        }
    }
}
