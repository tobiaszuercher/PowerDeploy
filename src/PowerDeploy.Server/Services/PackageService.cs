using System.Collections.Generic;
using System.Linq;

using PowerDeploy.Core;
using PowerDeploy.Server.Indexes;
using PowerDeploy.Server.Mapping;
using PowerDeploy.Server.Model;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Package;

using Raven.Client;

using ServiceStack;

namespace PowerDeploy.Server.Services
{
    public class PackageService : Service
    {
        public NugetServerBridge NugetServerBridge { get; set; }
        public NugetPackageDownloader PackageDownloader { get; set; }
        public IDocumentStore DocumentStore { get; set; }
        public IFileSystem FileSystem { get; set; }
        public ServerSettings ServerSettings { get; set; }

        public SynchronizePackageResponse Any(SynchronizePackageRequest request)
        {
            var summary = NugetServerBridge.Synchronize();

            return new SynchronizePackageResponse()
            {
                AddedPackages = summary.AddedPackages,
                TotalPackagesInNuget = summary.PackagesInNuget,
                TotalPackagesInPowerDeploy = summary.PackagesInPowerDeploy
            };
        }

        public PackageDto Get(GetPackageByIdRequest request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                return session.Load<Package>("packages/" + request.NugetId).ToDto();
            }
        }

        public List<PackageOverviewDto> Get(GetPackageRequest request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var packages = session.Query<Package>().ToList();

                return packages.Select(package => new PackageOverviewDto()
                {
                    NugetId = package.NugetId, 
                    Count = package.Versions.Count, 
                    LastVersion = package.Versions.OrderByDescending(p => p.Version).First().Version,
                    LastPublish = package.Versions.OrderByDescending(p => p.Version).First().Published,
                }).ToList();
            }
        }
    }
}