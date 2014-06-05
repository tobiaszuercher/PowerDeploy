using System;
using System.Collections.Generic;
using PowerDeploy.Server.ServiceModel.Package;

namespace PowerDeploy.Server.Model
{
    /// <summary>
    /// Metadata for a package from nuget which is stored in raven db.
    /// </summary>
    public class Package
    {
        public string Id { get; set; }
        public string NugetId { get; set; }
        public string Title { get; set; }

        public List<PackageVersion> Versions { get; set; }

        public Package()
        {
            Versions = new List<PackageVersion>();
        }

        public Package(string nugetId)
            : this()
        {
            Id = string.Format("packages/{0}", nugetId);
            NugetId = nugetId;
        }
    }

    public class PackageVersion
    {
        public string Version { get; set; }
        public DateTime Published { get; set; }
    }
}