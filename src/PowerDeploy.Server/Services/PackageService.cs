using System.Linq;
using PowerDeploy.Core;
using PowerDeploy.Server.Indexes;
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

        public PackageWithVersionDto Get(GetPackageByNugetIdRequest request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var packages = session.Query<Package>().Where(p => p.NugetId == request.NugetId).ToList();

                var response = new PackageWithVersionDto(request.NugetId);

                var deployed = session.Query<Deployment_Latest.ReducedResult, Deployment_Latest>()
                    .Where(d => d.NugetId == request.NugetId).ToList();

                foreach (var package in packages)
                {
                    var found = deployed.Where(p => p.Package.Version == package.Version).ToList();

                    var pv = new PackageVersionDto { Published = package.Published, Version = package.Version };

                    if (found.Any())
                    {
                        found.ForEach(f => pv.DeployedOn.Add(f.EnvironmentName));
                    }

                    response.Versions.Add(pv);
                }

                if (response.Versions.Any())
                {
                    response.LastVersion = response.Versions.OrderByDescending(p => p.Published).First().Version;
                    response.LastPublish = response.Versions.OrderByDescending(p => p.Published).First().Published;
                }

                return response;
            }
        }

        public object Get(GetPackagesRequest request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                return session.Query<Package_Overview.ReducedResult, Package_Overview>().ToList().Select(p => p.ConvertTo<PackageDto>());
            }
        }
    }
}