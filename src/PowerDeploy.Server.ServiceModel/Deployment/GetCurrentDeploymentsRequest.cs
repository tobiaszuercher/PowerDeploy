using System.Collections.Generic;

using ServiceStack;

namespace PowerDeploy.Server.ServiceModel.Deployment
{
    [Route("/deployments")]
    public class GetCurrentDeploymentsRequest : IReturn<List<DeploymentDto>>
    {
    }
}