using System;

namespace Arbor.NuGetServer.Api.Areas.Time
{
    public interface ICustomClock
    {
        DateTimeOffset UtcNow();
    }
}