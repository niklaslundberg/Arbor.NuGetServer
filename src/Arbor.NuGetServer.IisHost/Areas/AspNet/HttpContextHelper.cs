using System;
using System.Web;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public static class HttpContextHelper
    {
        public static Func<HttpContextBase> GetCurrentContext
        {
            get { return () => Wrapped(HttpContext.Current); }
        }

        private static HttpContextBase Wrapped(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }

            return new HttpContextWrapper(httpContext);
        }
    }
}
