using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    [Api("Trigger a deployment of a package by id.")]
    [Route("/package/deploy", Verbs = "POST")]
    [Route("/package/{id}/deploy", Verbs = "POST")]
    public class TriggerDeployment : IReturn<TriggerDeploymentResponse>
    {
        [ApiMember(Name = "PackageId", DataType = "int", ParameterType = "path")]
        public int PackageId { get; set; }

        //[ApiMember(Name = "EnvironmentName", DataType = "string", ParameterType = "query")]
        public string EnvironmentName { get; set; }
    }

    public class TriggerDeploymentResponse
    {
        
    }
}