using System;
using System.Linq;

using PowerDeploy.Server.ServiceModel;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace PowerDeploy.Server.Indexes
{
    public class Packages_ByNugetId : AbstractIndexCreationTask<Package>
    {
        public Packages_ByNugetId()
        {
            Map = packages => from package in packages select new { package.NugetId };

            Store(p => p.NugetId, FieldStorage.Yes);
            Store(p => p.Version, FieldStorage.Yes);
        }

        //////Reduce = packages => from package in packages
            //////                     group package by package.NugetId into g
            //////                     select new ReducedResult { NugetId = g.Key, Count = g.Sum(x => x.Count), LastPublished = g.Max(d => d.LastPublished) };

            ////Sort(pg => pg.NugetId, SortOptions.StringVal);
    }
}