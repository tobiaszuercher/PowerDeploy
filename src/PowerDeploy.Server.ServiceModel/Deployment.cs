using System;

namespace PowerDeploy.Server.ServiceModel
{
    /// <summary>
    /// Represents an executed deployment.
    /// </summary>
    public class Deployment
    {
        public string Id { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public string EnvironmentId { get; set; }
        public string PackageId { get; set; }
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
