namespace Arbor.NuGetServer.Api.Areas.NuGet.Clean
{
    public class CleanViewOutputModel
    {
        public CleanViewOutputModel(string cleanPostRoute, int packagesToKeep, bool preReleaseOnly, bool whatIf)
        {
            CleanPostRoute = cleanPostRoute;
            PackagesToKeep = packagesToKeep;
            PreReleaseOnly = preReleaseOnly;
            WhatIf = whatIf;
        }

        public string CleanPostRoute { get; }

        public int PackagesToKeep { get; }

        public bool PreReleaseOnly { get; }

        public bool WhatIf { get; }
    }
}
