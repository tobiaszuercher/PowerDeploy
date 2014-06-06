using System;
using System.Collections.Generic;
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
            public DateTime Published { get; set; }
            public string Version { get; set; }
            public int Count { get; set; }
        }

        public Packages_GroupedByNugetId()
        {
            Map = packages => from package in packages
                              select new ReducedResult
                              {
                                  NugetId = package.NugetId,
                                  Count = 1,
                                  Published = package.Published,
                                  Version = package.Version
                              };

            Reduce = packages => from package in packages
                                 group package by package.NugetId into g
                                 select new ReducedResult
                                 {
                                     NugetId = g.Key,
                                     Count = g.Sum(x => x.Count),
                                     Published = g.Max(d => d.Published),
                                     Version = g.First().Version,
                                 };

            Sort(pg => pg.NugetId, SortOptions.StringVal);
        }
    }

    //public class HomoIndex : AbstractIndexCreationTask<Package>
    //{
    //    public HomoIndex()
    //    {
    //        //Map => packages => from package in packages
    //        //                    select new {  }
    //    }
    //}
}