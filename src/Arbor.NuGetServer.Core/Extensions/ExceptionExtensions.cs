using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Arbor.NuGetServer.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static bool IsFatal(this Exception ex)
        {
            if (ex == null)
            {
                return false;
            }

            return ex is StackOverflowException || ex is OutOfMemoryException || ex is AccessViolationException
                   || ex is AppDomainUnloadedException || ex is ThreadAbortException || ex is SEHException;
        }

        public static T DeepestExceptionOfType<T>(this Exception ex) where T : Exception
        {
            if (ex == null)
            {
                return null;
            }

            var deepestExceptionOfType = DeepestExceptionOfType<T>(ex.InnerException);

            if (deepestExceptionOfType == null)
            {
                if (ex is T exception)
                {
                    return exception;
                }
            }

            return deepestExceptionOfType;
        }
    }
}
