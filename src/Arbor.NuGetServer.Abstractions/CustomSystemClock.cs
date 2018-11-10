using System;

namespace Arbor.NuGetServer.Abstractions
{
    public class CustomSystemClock : ICustomClock
    {
        public DateTimeOffset UtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
