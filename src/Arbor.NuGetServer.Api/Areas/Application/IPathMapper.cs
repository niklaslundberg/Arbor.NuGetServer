using System;

namespace Arbor.NuGetServer.Api.Areas.Application
{
    public delegate string MapPath(string relativePath);

    public class Functions
    {
        public MapPath MapPath { get; set; } = relativePath => relativePath;
    }
}
