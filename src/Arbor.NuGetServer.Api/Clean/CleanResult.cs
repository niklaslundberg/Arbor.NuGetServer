using System;
using System.Collections.Generic;

namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanResult
    {
        public IReadOnlyCollection<CleanedPackage> CleanedPackages { get; }

        public CleanResult(IReadOnlyCollection<CleanedPackage> cleanedPackages)
        {
            CleanedPackages = cleanedPackages;
        }
        private CleanResult()
        {
            CleanedPackages = Array.Empty<CleanedPackage>();
        }

        public static CleanResult NotRun => new CleanResult();
    }
}