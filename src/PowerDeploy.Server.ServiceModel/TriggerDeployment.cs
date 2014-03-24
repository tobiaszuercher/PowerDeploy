using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    [Api("Trigger a deployment of a package by id.")]
    [Route("/package/deploy", Verbs = "POST")]
    [Route("/package/{packageid}/{version}/deploy", Verbs = "POST")]
    public class TriggerDeployment : IReturn<TriggerDeploymentResponse>
    {
        [ApiMember(Name = "PackageId", DataType = "string", ParameterType = "path")]
        public string PackageId { get; set; }

        [ApiMember(Name = "Version", DataType = "string", ParameterType = "path")]
        public string Version { get; set; }

        [ApiMember(Name = "EnvironmentName", DataType = "string", ParameterType = "query")]
        public string Environment { get; set; }
    }

    public class TriggerDeploymentResponse
    {
        
    }
}