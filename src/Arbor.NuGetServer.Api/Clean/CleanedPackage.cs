using System;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanedPackage
    {
        public CleanedPackage([NotNull] string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fullName));
            }

            FullName = fullName;
        }

        public string FullName { get; }
    }
}
