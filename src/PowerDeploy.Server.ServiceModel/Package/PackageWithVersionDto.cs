using System;
using System.Collections.Generic;

namespace PowerDeploy.Server.ServiceModel.Package
{
    /// <summary>
    /// Metadata for a package from nuget which is stored in raven db.
    /// </summary>
    public class PackageWithVersionDto// : IComparable
    {
        public string Uri { get; set; }
        public string NugetId { get; set; }
        public DateTime LastPublish { get; set; }
        public string LastVersion { get; set; }

        public List<PackageVersionDto> Versions { get; set; }

        public PackageWithVersionDto()
        {
            Versions = new List<PackageVersionDto>();
        }

        public PackageWithVersionDto(string nugetId)
            : this()
        {
            NugetId = nugetId;
        }
    }

    public class PackageOverviewDto
    {
        public string NugetId { get; set; }
        public int Count { get; set; }
        public DateTime LastPublish { get; set; }
        public string LastVersion { get; set; }
    }

    public class PackageVersionDto
    {
        public string Version { get; set; }
        public DateTime Published { get; set; }
        public long Size { get; set; }
        public List<string> DeployedOn { get; set; }

        public PackageVersionDto()
        {
            DeployedOn = new List<string>();
        }
    }
}