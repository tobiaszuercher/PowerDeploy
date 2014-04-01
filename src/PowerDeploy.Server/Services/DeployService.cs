using PowerDeploy.Core;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;

using ServiceStack;

namespace PowerDeploy.Server.Services
{
    public class DeployService : Service
    {
        public IFileSystem FileSystem { get; set; }
        public ServerSettings ServerSettings { get; set; }
        public NugetPackageDownloader PackageDownloader { get; set; }

        public TriggerDeploymentResponse Post(TriggerDeployment request)
        {
            using (var workspace = new Workspace(FileSystem, ServerSettings))
            {
                var neutralPackagePath = PackageDownloader.Downloaad(request.PackageId, request.Version, workspace.TempWorkDir);

                workspace.UpdateSources();

                var packageManager = new PackageManager(workspace.EnviornmentPath);
                var configuredPackage = packageManager.ConfigurePackageByEnvironment(neutralPackagePath, request.Environment, workspace.TempWorkDir);
                packageManager.DeployPackage(configuredPackage);

                return new TriggerDeploymentResponse();
            }
        } 
    }
}