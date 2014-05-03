using System;
using System.Collections.Generic;

using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    public class PackageGroup
    {
        public string NugetId { get; set; }
        public DateTime LastPublished { get; set; }
        public int Count { get; set; }
    }

    [Route("/package-groups", Verbs = "GET")]
    public class QueryPackageGroup : IReturn<List<PackageGroup>>
    {
        /// <summary>
        /// Filter for NugetId, leave empty for no filter.
        /// </summary>
        public string NugetId { get; set; }
    }
}