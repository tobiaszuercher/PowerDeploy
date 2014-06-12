using System;
using PowerDeploy.Server.ServiceModel.Environment;
using PowerDeploy.Server.ServiceModel.Package;

using ServiceStack;

namespace PowerDeploy.Server.ServiceModel.Deployment
{
    [Route("/deployments")]
    public class DeploymentDto
    {
        public string Uri { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public EnvironmentDto Environment { get; set; }
        public PackageDto Package { get; set; }
        public DeployStatus Status { get; set; }
    }

    //public class Deployment
    //{
    //    public string Id { get; set; }
    //    public DateTime RequestedAt { get; set; }
    //    public DateTime FinishedAt { get; set; }
    //    public string EnvironmentId { get; set; }
    //    public Package Package { get; set; }
    //    public DeployStatus Status { get; set; }

    //    public Deployment()
    //    {
    //        Package = new Package();
    //    }
    //}

    public enum DeployStatus
    {
        Requested,
        Deploying,
        Successful,
        Failed
    }
}
