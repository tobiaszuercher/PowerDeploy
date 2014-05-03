using Raven.Client;

using ServiceStack;

namespace PowerDeploy.Server.Services
{
    public class DeploymentStatusService : Service
    {
        public IDocumentStore DocumentStore { get; set; }
    }
}
