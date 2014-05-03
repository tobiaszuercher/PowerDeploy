using System;

namespace PowerDeploy.Server.ServiceModel
{
    /// <summary>
    /// Represents an executed deployment.
    /// </summary>
    public class DeploymentDto
    {
        public string Url { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public string EnvironmentName { get; set; }
        public string EnvironmentUrl { get; set; }
        
        public DeployStatus Status { get; set; }
    }

    public enum DeployStatus
    {
        Requested,
        Deploying,
        Successful,
        Failed
    }
}
