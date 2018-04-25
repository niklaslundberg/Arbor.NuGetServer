using System;

namespace Arbor.NuGetServer.IisHost.Html
{
    public static class HtmlAttributes
    {
        public static string Checked(this bool enabled)
        {
            if (!enabled)
            {
                return String.Empty;
            }

            return "checked=\"checked\"";
        }
    }
}
