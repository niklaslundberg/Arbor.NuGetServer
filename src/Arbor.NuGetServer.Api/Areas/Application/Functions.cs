using System;

namespace Arbor.NuGetServer.Api.Areas.Application
{
    public class Functions
    {
        public MapPath MapPath { get; set; } = relativePath => relativePath;
    }
}
