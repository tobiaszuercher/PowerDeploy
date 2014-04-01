using System.IO;
using System.Net;

using PowerDeploy.Core;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;

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
                    return session.Query<PackageInfo>();
                }

                return session.Query<PackageInfo>("PackageInfos/" + request.Id);
            }
        }
    }
}