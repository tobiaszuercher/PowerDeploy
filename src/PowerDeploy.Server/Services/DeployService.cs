using System;
using System.Linq;

using PowerDeploy.Core;
using PowerDeploy.Server.Model;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;

using Raven.Client;

using ServiceStack;

using Environment = PowerDeploy.Server.Model.Environment;

namespace PowerDeploy.Server.Services
{
    public class DeployService : Service
    {
        public IFileSystem FileSystem { get; set; }
        public ServerSettings ServerSettings { get; set; }
        public NugetPackageDownloader PackageDownloader { get; set; }
        public IDocumentStore DocumentStore { get; set; }

        public TriggerDeploymentResponse Post(TriggerDeployment request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var environment = session.Query<Environment>().FirstOrDefault(e => e.Name.ToUpperInvariant() == request.Environment.ToUpperInvariant());

                if (environment == null)
                {
                    throw HttpError.NotFound("Environment {0} not found.".Fmt(request.Environment));
                }
                
                var deploymentInfo = new Deployment()
                {
                    EnvironmentId = "environments/" + environment.Id,
                    RequestedAt = DateTime.UtcNow,
                };

                var packageInfo = session.Load<Package>("packages/{0}/{1}".Fmt(request.PackageId, request.Version));

                deploymentInfo.PackageId = packageInfo.Id;
                deploymentInfo.Status = DeployStatus.Deploying;
                session.Store(deploymentInfo);

                session.SaveChanges();

                using (var workspace = new Workspace(FileSystem, ServerSettings))
                {
                    var neutralPackagePath = PackageDownloader.Downloaad(request.PackageId, request.Version, workspace.TempWorkDir);

                    workspace.UpdateSources();

                    var packageManager = new PackageManager(workspace.EnviornmentPath);
                    var configuredPackage = packageManager.ConfigurePackageByEnvironment(neutralPackagePath, request.Environment, workspace.TempWorkDir);
                    packageManager.DeployPackage(configuredPackage);
                    deploymentInfo.FinishedAt = DateTime.UtcNow;
                    deploymentInfo.Status = DeployStatus.Successful;
                    session.SaveChanges();

                    return new TriggerDeploymentResponse();
                }
            }
        } 
    }
}