using System.IO;
using System.Linq;
using System.Net;

using PowerDeploy.Core;
using PowerDeploy.Server.Indexes;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Package;

using Raven.Client;

using ServiceStack;

namespace PowerDeploy.Server.Services
{
    public class PackageService : Service
    {
        public PackageProvider PackageProvider { get; set; }
        public NugetPackageDownloader PackageDownloader { get; set; }
        public IDocumentStore DocumentStore { get; set; }
        public IFileSystem FileSystem { get; set; }
        public ServerSettings ServerSettings { get; set; }

        public SynchronizePackageResponse Any(SynchronizePackageRequest request)
        {
            var summary = PackageProvider.Synchronize();

            return new SynchronizePackageResponse().PopulateWith(summary);
        }

        public object Get(QueryPackageInfo request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                if (request.Id == default(int))
                {
                    return session.Query<PackageDto>();
                }

                return session.Query<PackageDto>("PackageInfos/" + request.Id);
            }
        }

        public object Get(QueryPackageGroup request)
        {
            //using (var session = DocumentStore.OpenSession())
            //{
            //    if (string.IsNullOrEmpty(request.NugetId))
            //    {
            //        return session.Query<PackageGroup, PackageGroup_ByPackageNugetId>().ToList();
            //    }

            //    return session.Query<PackageGroup, PackageGroup_ByPackageNugetId>().Where(pg => pg.NugetId == request.NugetId);
            //}

            return null;
        }
    }
}