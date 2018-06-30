using System;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public class FeedId
    {
        public string Feed { get; }

        public FeedId([NotNull] string feed)
        {
            if (string.IsNullOrWhiteSpace(feed))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(feed));
            }

            Feed = feed;
        }
    }
}