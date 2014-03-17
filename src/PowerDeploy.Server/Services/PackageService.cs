using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;

using ServiceStack;

namespace PowerDeploy.Server.Services
{
    public class PackageService : Service
    {
        public PackageProvider PackageProvider { get; set; }

        public SynchronizePackageResponse Any(SynchronizePackageRequest request)
        {
            var summary = PackageProvider.Synchronize();

            return new SynchronizePackageResponse().PopulateWith(summary);
        }
    }
}