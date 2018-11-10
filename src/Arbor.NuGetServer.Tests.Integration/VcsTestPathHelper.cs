
using Alphaleonis.Win32.Filesystem;
using Arbor.Aesculus.Core;
using NCrunch.Framework;

namespace Arbor.NuGetServer.Tests.Integration
{
    public static class VcsTestPathHelper
    {
        public static string FindVcsRootPath()
        {
            if (NCrunchEnvironment.NCrunchIsResident())
            {
                var originalSolutionFile = new FileInfo(NCrunchEnvironment.GetOriginalSolutionPath());

                string rootPath = VcsPathHelper.FindVcsRootPath(originalSolutionFile.Directory?.FullName);

                return rootPath;
            }

            return VcsPathHelper.FindVcsRootPath();
        }
    }
}
