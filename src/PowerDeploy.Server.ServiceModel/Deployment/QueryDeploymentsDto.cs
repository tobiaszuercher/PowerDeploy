using System.Collections.Generic;

using ServiceStack;

namespace PowerDeploy.Server.ServiceModel.Deployment
{
    [Route("/deployments")]
    public class QueryDeploymentsDto : IReturn<List<DeploymentDto>>
    {
        public string Environment { get; set; }
        public string Package { get; set; }
    }
}