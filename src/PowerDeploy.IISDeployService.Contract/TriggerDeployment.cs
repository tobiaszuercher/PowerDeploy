using System.Runtime.Serialization;

using ServiceStack.ServiceHost;

namespace PowerDeploy.IISDeployService.Contract
{
    [Api("Trigger a deployment")]
    [Route("/deployments", Verbs = "POST") ]
    public class TriggerDeployment : IReturn<TriggerDeploymentResponse>
    {
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }
        public string AppPoolName { get; set; }
        public string AppPoolUser { get; set; }
        public string AppPoolPassword { get; set; }
        public string WebsiteName { get; set; }
        public string WebsitePhysicalPath { get; set; }
        public string AppRoot { get; set; }
        public string AppName { get; set; }
        public string AppPhysicalPath { get; set; }
        public int WebsitePort { get; set; }
        public RuntimeVersion RuntimeVersion { get; set; }
        public bool Overwrite { get; set; }
    }

    public enum RuntimeVersion
    {
        [EnumMember(Value = "1.1")]
        Version11,

        [EnumMember(Value = "2.0")]
        Version20,

        [EnumMember(Value = "4.0")]
        Version40,

        [EnumMember(Value = "4.5")]
        Version45,
    }

    public class TriggerDeploymentResponse
    {
        public string Status { get; set; }
    }
}