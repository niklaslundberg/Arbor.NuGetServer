using System.Threading.Tasks;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core.Extensions;

namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanService
    {
        public async Task<CleanResult> CleanAsync()
        {
            if (!KVConfigurationManager.AppSettings[CleanConstants.CleanEnabled].ParseAsBoolOrDefault())
            {
                return CleanResult.NotRun;
            }

            return new CleanResult();
        }
    }
}