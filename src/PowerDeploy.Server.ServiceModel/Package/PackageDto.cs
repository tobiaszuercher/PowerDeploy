using System;
using System.Collections.Generic;

namespace PowerDeploy.Server.ServiceModel.Package
{
    /// <summary>
    /// Metadata for a package from nuget which is stored in raven db.
    /// </summary>
    public class PackageDto// : IComparable
    {
        public string Uri { get; set; }
        public string Id { get; set; }
        public string NugetId { get; set; }

        public List<PackageVersionDto> Versions { get; set; }

        public PackageDto()
        {
            Versions = new List<PackageVersionDto>();
        }

        public PackageDto(string nugetId)
            : this()
        {
            Id = string.Format("packages/{0}", nugetId);
            NugetId = nugetId;
        }

        //public int CompareTo(object other)
        //{
        //    var otherVersion = new Version(((PackageDto)other).Version);

        //    return new Version(Version).CompareTo(otherVersion);
        //}
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
    }
}