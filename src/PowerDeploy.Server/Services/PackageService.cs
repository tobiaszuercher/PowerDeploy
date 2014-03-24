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

        public TriggerDeploymentResponse Post(TriggerDeployment request)
        {
            var tempDir = FileSystem.CreateTempWorkingDir(ServerSettings.WorkDir);
            var neutralPackagePath = Path.Combine(tempDir, "{0}_v{1}.nupkg".Fmt(request.PackageId, request.Version));

            using (var webclient = new WebClient())
            {
                webclient.DownloadFile(
                    "{0}/api/v2/package/{1}/{2}".Fmt(ServerSettings.NuGetServerUri, request.PackageId, request.Version),
                    neutralPackagePath);
            }

            FileSystem.EnsureDirectoryExists(ServerSettings.WorkDir);

            // todo git/tfs decision
            var repoDir = Path.Combine(ServerSettings.WorkDir, "repo");
            var git = new GitWrapper(repoDir);
            git.PullOrCloneRepository(ServerSettings.RepositoryUrl);

            var envPath = Path.Combine(repoDir, ServerSettings.EnvironmentsPath);

            var packageManager = new PackageManager(envPath);
            var configuredPackage = packageManager.ConfigurePackageByEnvironment(neutralPackagePath, request.Environment, tempDir);
            packageManager.DeployPackage(configuredPackage);

            FileSystem.DeleteTempWorkingDirs();

            return new TriggerDeploymentResponse();
            //var m = new PackageManager();
            //m.ConfigurePackage("neutral_package.nupkg", "/env/dev.xml", "/work/");
            //m.DeployPackage("configured_package.nupkg");
        }
    }
}