using ServiceStack.ServiceHost;

namespace PowerDeploy.IISDeployService.Contract
{
    [Route("/hello")]
    public class Hello : IReturn<string>
    {
        public string Name { get; set; }
    }
}