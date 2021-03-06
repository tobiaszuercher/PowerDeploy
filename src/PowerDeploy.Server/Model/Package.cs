﻿using System;

namespace PowerDeploy.Server.Model
{
    /// <summary>
    /// Metadata for a package from nuget which is stored in raven db.
    /// </summary>
    public class Package //: IComparable
    {
        public string Id { get; set; }
        public string NugetId { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Authors { get; set; }
        public string IconUrl { get; set; }
        public string Tags { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime Published { get; set; }
        public long PackageSize { get; set; }

        public Package()
        {
        }

        public Package(string nugetId, string version)
        {
            Id = string.Format("packages/{0}/{1}", nugetId, version);
            NugetId = nugetId;
            Version = version;
        }

        ////public int CompareTo(object other)
        ////{
        ////    var otherVersion = new Version(((PackageDto)other).Version);

        ////    return new Version(Version).CompareTo(otherVersion);
        ////}
    }
}