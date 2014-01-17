using System;

namespace PowerDeploy.Dashboard.DataAccess.Entities
{
    public class Package : IComparable
    {
        public int Id { get; set; }
        public string NugetId { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Authors { get; set; }
        public string IconUrl { get; set; }
        public string Tags { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime Published { get; set; }
        ////public int DownloadCount { get; set; }
        public long PackageSize { get; set; }

        public int CompareTo(object other)
        {
            var otherVersion = new Version(((Package)other).Version);

            return new Version(Version).CompareTo(otherVersion);
        }
    }
}