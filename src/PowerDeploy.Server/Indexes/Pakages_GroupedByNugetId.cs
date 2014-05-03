using System;
using System.Linq;

using PowerDeploy.Server.Model;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace PowerDeploy.Server.Indexes
{
    public class Packages_GroupedByNugetId : AbstractIndexCreationTask<Package, Packages_GroupedByNugetId.ReducedResult>
    {
        public class ReducedResult
        {
            public string NugetId { get; set; }
            public DateTime LastPublished { get; set; }
            public int Count { get; set; }
        }

        public Packages_GroupedByNugetId()
        {
            Map = packages => from package in packages select new ReducedResult { NugetId = package.NugetId, Count = 1, LastPublished = package.Published };

            Reduce = packages => from package in packages
                                 group package by package.NugetId into g
                                 select new ReducedResult { NugetId = g.Key, Count = g.Sum(x => x.Count), LastPublished = g.Max(d => d.LastPublished) };

            Sort(pg => pg.NugetId, SortOptions.StringVal);
        }
    }
}