using System.IO;
using Path = Arbor.Ginkgo.Path;

namespace Arbor.NuGetServer.Tests.Integration
{
    public static class TestHelper
    {
        public static void BeforeStart(Path websiteRootPath)
        {
            string vcsRootPath = VcsTestPathHelper.FindVcsRootPath();

            Path settingsJsonPath = Path.Combine(vcsRootPath,
                "src",
                "Arbor.NuGetServer.Tests.Integration",
                "settings.json");

            var fileInfo = new FileInfo(settingsJsonPath.FullName);

            if (fileInfo.Exists)
            {
                var targetDirectory = new DirectoryInfo(Path.Combine(websiteRootPath, "App_Data").FullName);

                if (!targetDirectory.Exists)
                {
                    targetDirectory.Create();
                }

                Path targetFile = Path.Combine(targetDirectory.FullName, fileInfo.Name);

                fileInfo.CopyTo(targetFile.FullName);
            }
        }
    }
}
