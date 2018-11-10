using System;

namespace Arbor.NuGetServer.Api.Areas.Time
{
    public class CustomSystemClock : ICustomClock
    {
        public DateTimeOffset UtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
