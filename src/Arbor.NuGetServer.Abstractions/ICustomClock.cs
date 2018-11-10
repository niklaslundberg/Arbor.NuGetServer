using System;

namespace Arbor.NuGetServer.Abstractions
{
    public interface ICustomClock
    {
        DateTimeOffset UtcNow();
    }
}