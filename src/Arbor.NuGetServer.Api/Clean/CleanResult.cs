using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanResult
    {
        public CleanResult([NotNull] IReadOnlyCollection<CleanedPackage> cleanedPackages, int packagesToKeep)
        {
            CleanedPackages = cleanedPackages ?? throw new ArgumentNullException(nameof(cleanedPackages));
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
