using ServiceStack.ServiceHost;

namespace PowerDeploy.DeploymentService.Contract
{
    [Route("/environments")]
    public class QueryEnvironment : IReturn<Environment>
    {
    }
}
