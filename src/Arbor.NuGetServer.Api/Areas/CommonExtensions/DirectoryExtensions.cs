﻿using System;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.CommonExtensions
{
    public static class DirectoryExtensions
    {
        public static DirectoryInfo EnsureExists([NotNull] this DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            directoryInfo.Refresh();

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                directoryInfo.Refresh();
            }

            return directoryInfo;
        }
    }
}
