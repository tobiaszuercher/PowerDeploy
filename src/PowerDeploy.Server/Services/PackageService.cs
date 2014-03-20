using System;
using System.IO;
using System.Linq;
using System.Net;

using PowerDeploy.Core;
using PowerDeploy.Server.NuGetServer;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;

using Raven.Client;

using ServiceStack;
using ServiceStack.Logging;

namespace PowerDeploy.Server.Services
{
    public class PackageService : Service
    {
        public PackageProvider PackageProvider { get; set; }
        public IDocumentStore DocumentStore { get; set; }

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
            // get package from nuget
            using (var webclient = new WebClient())
            {
                webclient.DownloadFile("http://localhost/Nuggy/api/v2/package/powerdeploy.sample.xcopy/1.0.0.12", @"c:\temp\powerdeploy.sample.xcopy_v0.0.3.18_svc.nupkg");
            }

            using (var session = DocumentStore.OpenSession())
            {
                var serverSettings = session.Load<ServerSettings>("ServerSettings/1");
                var environment = session.Load<ServiceModel.Environment>("Environments/" + request.EnvironmentName);

                var fs = new PhysicalFileSystem();
                fs.EnsureDirectoryExists(serverSettings.WorkDir);
                var workDir = fs.CreateTempWorkingDir(serverSettings.WorkDir);

                // todo git/tfs decision
                var repoDir = Path.Combine(workDir, "repo");
                var git = new GitWrapper(repoDir);
                git.PullOrCloneRepository(serverSettings.RepositoryUrl);

                var envPath = Path.Combine(repoDir, serverSettings.EnvironmentsPath);

                var configureDir = Path.Combine(workDir, "configured");
                fs.EnsureDirectoryExists(configureDir);

                var packageManager = new PackageManager(envPath);
                var configuredPackage = packageManager.ConfigurePackageByEnvironment(@"c:\temp\powerdeploy.sample.xcopy_v0.0.3.18_svc.nupkg", request.EnvironmentName, configureDir);
                packageManager.DeployPackage(configuredPackage);

                fs.DeleteTempWorkingDirs();
            }

            return new TriggerDeploymentResponse();
            //var m = new PackageManager();
            //m.ConfigurePackage("neutral_package.nupkg", "/env/dev.xml", "/work/");
            //m.DeployPackage("configured_package.nupkg");
        }
    }
}