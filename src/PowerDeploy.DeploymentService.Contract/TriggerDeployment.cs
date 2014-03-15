using ServiceStack;

namespace PowerDeploy.DeploymentService.Contract
{
    [Route("/deployment/trigger", Verbs = "POST")]
    public class TriggerDeployment : IReturn<TriggerDeploymentResponse>
    {
        [ApiMember(Name = "Name", DataType = "string", ParameterType = "query")]
        public string Name { get; set; }
    }

    public class TriggerDeploymentResponse
    {
        
    }

    public class TriggerDeploymentMessageQueue : IReturn<TriggerDeploymentMessageQueueResponse>
    {
        public string Name { get; set; }
    }

    public class TriggerDeploymentMessageQueueResponse
    {
        public bool Success { get; set; }
    }
}