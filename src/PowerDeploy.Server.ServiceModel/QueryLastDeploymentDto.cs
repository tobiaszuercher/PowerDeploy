using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    [Route("/deployments/latest", Verbs = "GET")]
    public class QueryLastDeploymentDto : IReturn<DeploymentDto>
    {

    }
}
