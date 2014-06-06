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

        public object Get(GetPackageOverviewRequest request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                return session.Query<Package_Overview.ReducedResult, Package_Overview>().ToList().Select(p => p.ConvertTo<PackageOverviewDto>());                
            }
        }
    }
}