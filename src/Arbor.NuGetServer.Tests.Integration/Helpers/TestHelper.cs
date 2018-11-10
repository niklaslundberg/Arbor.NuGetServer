using Alphaleonis.Win32.Filesystem;
using Path = Arbor.Ginkgo.Path;

namespace Arbor.NuGetServer.Tests.Integration.Helpers
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

            Path binDirectoryPath = Path.Combine(websiteRootPath, "bin");

            var binDirectory = new DirectoryInfo(binDirectoryPath.FullName);

            var assemblyFile = new FileInfo(typeof(TestHelper).Assembly.Location);

            foreach (FileInfo dllFile in assemblyFile.Directory.GetFiles("*.dll"))
            {
                Path targetFile = Path.Combine(binDirectory.FullName, dllFile.Name);

                if (!targetFile.Exists)
                {
                    //dllFile.CopyTo(targetFile.FullName);
                }
            }
        }
    }
}
