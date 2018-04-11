using System;
using System.Collections.Generic;

namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanResult
    {
        public CleanResult(IReadOnlyCollection<CleanedPackage> cleanedPackages, int packagesToKeep)
        {
            CleanedPackages = cleanedPackages;
            PackagesToKeep = packagesToKeep;
        }

        private CleanResult()
        {
            CleanedPackages = Array.Empty<CleanedPackage>();
        }

        public IReadOnlyCollection<CleanedPackage> CleanedPackages { get; }

        public int PackagesToKeep { get; }

        public static CleanResult NotRun => new CleanResult();
    }
}
