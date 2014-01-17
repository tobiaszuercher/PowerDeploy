using System.IO;

using NuGet;

using PowerDeploy.Core.Extensions;
using PowerDeploy.Core.Logging;
using PowerDeploy.IISDeployService.Contract;

using ServiceStack.ServiceClient.Web;

namespace PowerDeploy.Core.Deploy
{
    namespace PowerDeploy.Core.Deploy
    {
        [DeployerMetadata(PackageType = "iis")]
        public class WebDeployer : IDeployer
        {
            public WebDeployer()
            {
            }

            public bool Deploy(ZipPackage package, ILog logger)
            {
                var options = package.GetPackageOptions<WebOptions>();
                
                logger.Info("Deploying iis package to {0}:{1} using {2}".Fmt(options.WebsiteName, options.WebsitePort, options.DeployService));

                var client = new JsonServiceClient(options.DeployService); // todo
                var request = new TriggerDeployment()
                {
                    AppPoolName = "ZZZ_Integration_PoolName",
                    AppPoolUser = "JackBauer",
                    AppPoolPassword = "topsecret",
                    WebsiteName = "ZZZ_Integration_Website_Simple",
                    AppRoot = "/",
                    PackageId = "IntegrationTest",
                    PackageVersion = "1.3.3.7",
                    WebsitePhysicalPath = @"C:\temp\integrationtests",
                    RuntimeVersion = RuntimeVersion.Version40,
                };

                var response = client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("package.zip"), request);

                return true;
            }
        }
    }
}