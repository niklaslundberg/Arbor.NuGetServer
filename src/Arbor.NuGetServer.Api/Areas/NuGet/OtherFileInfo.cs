using System;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet
{
    public class OtherFileInfo
    {
        public OtherFileInfo([NotNull] FileInfo fileInfo, [NotNull] string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(relativePath));
            }

            FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            RelativePath = relativePath;
        }

        public FileInfo FileInfo { get; }

        public string RelativePath { get; }
    }
}
