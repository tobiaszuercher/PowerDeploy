using System.Collections.Generic;
using System.Linq;

using PowerDeploy.Core;
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

            return new SynchronizePackageResponse().PopulateWith(summary);
        }

        public object Get(QueryPackageDto request)
        {
            // todo add validator for version & nugetid.
            using (var session = DocumentStore.OpenSession())
            {
                return session.Load<Package>("packages/{0}/{1}".Fmt(request.NugetId, request.Version)).ToDto();
            }
        }

        public List<PackageDto> Get(QueryPackagesDto request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                if (string.IsNullOrEmpty(request.NugetId))
                {
                    return session.Query<Package>().OrderByDescending(p => p.Published).ToList().Select(p => p.ToDto()).ToList();
                }

                return session.Query<Package>()
                                .Where(p => p.NugetId == request.NugetId)
                                .OrderByDescending(p => p.Published)
                                .ToList()
                                .Select(p => p.ToDto())
                                .ToList();
            }
        }
    }
}