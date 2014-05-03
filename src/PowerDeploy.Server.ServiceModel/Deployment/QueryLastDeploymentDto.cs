using ServiceStack;

namespace PowerDeploy.Server.ServiceModel.Deployment
{
    [Route("/deployments/latest", Verbs = "GET")]
    public class QueryLastDeploymentDto : IReturn<DeploymentDto>
    {

    }
}
