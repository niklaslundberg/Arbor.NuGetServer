using System;
using System.Web.Mvc;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters([NotNull] GlobalFilterCollection filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            filters.Add(new HandleErrorAttribute());
        }
    }
}
