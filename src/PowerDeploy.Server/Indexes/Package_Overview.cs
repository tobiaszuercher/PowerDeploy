using System;
using System.Linq;
using PowerDeploy.Server.Model;
using Raven.Client.Indexes;

namespace PowerDeploy.Server.Indexes
{
    public class Package_Overview : AbstractIndexCreationTask<Package, Package_Overview.ReducedResult>
    {
        public class ReducedResult
        {
            public string NugetId { get; set; }
            public int Count { get; set; }
            public DateTime LastPublish { get; set; }
            public string LastVersion { get; set; }
        }

        public Package_Overview()
        {
            Map = packages => from package in packages
                              from version in package.Versions
                                            select new ReducedResult()
                                            {
                                                NugetId = package.NugetId,
                                                Count = 1,
                                                LastPublish = version.Published,
                                                LastVersion = version.Version,
                                            };

            Reduce = results => from result in results
                group result by result.NugetId
                into g
                select new ReducedResult()
                {
                    NugetId = g.Key,
                    Count = g.Sum(r => r.Count),
                    LastVersion = g.OrderByDescending(r => r.LastPublish).First().LastVersion,
                    LastPublish = g.OrderByDescending(r => r.LastPublish).First().LastPublish,
                };
        }
    }
}