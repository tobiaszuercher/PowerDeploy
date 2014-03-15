using ServiceStack;

namespace PowerDeploy.DeploymentService.Contract
{
    [Route("/environments")]
    public class QueryEnvironment : IReturn<Environment>
    {
    }
}
