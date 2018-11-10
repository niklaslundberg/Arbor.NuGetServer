using System;
using System.Web;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public static class HttpContextHelper
    {
        public static Func<HttpContextBase> GetCurrentContext
        {
            get { return () => Wrapped(HttpContext.Current); }
        }

        private static HttpContextBase Wrapped(HttpContext httpContext)
        {
            return httpContext is null ? null : new HttpContextWrapper(httpContext);
        }
    }
}
