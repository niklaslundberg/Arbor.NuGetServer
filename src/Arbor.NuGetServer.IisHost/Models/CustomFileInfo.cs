using System;
using System.IO;

namespace Arbor.NuGetServer.IisHost.Models
{
    public class CustomFileInfo
    {
        public CustomFileInfo(FileInfo fileInfo, string relativePath)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(relativePath));
            }

            FileInfo = fileInfo;
            RelativePath = relativePath;
        }

        public FileInfo FileInfo { get; }

        public string RelativePath { get; }
    }
}
