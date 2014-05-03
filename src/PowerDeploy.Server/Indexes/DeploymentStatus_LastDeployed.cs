using System;
using System.Linq;

using PowerDeploy.Server.ServiceModel;

using Raven.Client.Indexes;

using Environment = PowerDeploy.Server.ServiceModel.Environment;

namespace PowerDeploy.Server.Indexes
{
    public class DeploymentStatus_LastDeployed : AbstractIndexCreationTask<Deployment, DeploymentStatus_LastDeployed.ReducedResult>
    {
        public class ReducedResult
        {
            public string Id { get; set; }
            public DateTime LastRequestedAt { get; set; }
            public DateTime LastFinishedAt { get; set; }
            public string EnvironmentId { get; set; }
            public string EnvironmentName { get; set; }
            public string PackageId { get; set; }
            public string PackageName { get; set; }
            public string PackageVersion { get; set; }
            public int Deployments { get; set; }
            public DeployStatus Status { get; set; }
        }

        public DeploymentStatus_LastDeployed()
        {
            Map = deployments => from deployment in deployments 
                                 select new ReducedResult()
                                 {
                                     Id = deployment.Id, 
                                     LastFinishedAt = deployment.FinishedAt, 
                                     LastRequestedAt = deployment.RequestedAt,
                                     EnvironmentId = deployment.EnvironmentId,
                                     EnvironmentName = LoadDocument<Environment>(deployment.EnvironmentId).Name,
                                     PackageName = LoadDocument<Package>(deployment.PackageId).NugetId,
                                     PackageVersion = LoadDocument<Package>(deployment.PackageId).Version,
                                     PackageId = deployment.PackageId,
                                     Deployments = 1,
                                 };


            Reduce = deployments => from deployment in deployments
                group deployment by new { deployment.PackageName, deployment.EnvironmentId }
                into g
                select
                    new ReducedResult()
                    {
                        EnvironmentId = g.Max(d => d.EnvironmentId),
                        EnvironmentName = g.Max(d => d.EnvironmentName),
                        Deployments = g.Sum(d => d.Deployments),
                        LastFinishedAt = g.Max(d => d.LastFinishedAt),
                        LastRequestedAt = g.Max(d => d.LastRequestedAt),
                        PackageId = g.Max(d => d.PackageId),
                        PackageVersion = g.Max(d => d.PackageVersion),
                        Id = g.Max(d => d.Id),
                        PackageName = g.Max(p => p.PackageName),
                    };
        }
    }

    public class DeploymentStatus_ByEnvironment : AbstractIndexCreationTask<Deployment>
    {
        public class ReducedResult
        {
            public string Id { get; set; }
            public DateTime LastRequestedAt { get; set; }
            public DateTime LastFinishedAt { get; set; }
            public string EnvironmentId { get; set; }
            public string EnvironmentName { get; set; }
            public string PackageName { get; set; }
            public string PackageId { get; set; }
            public int Deployments { get; set; }
            public DeployStatus Status { get; set; }
        }

        public DeploymentStatus_ByEnvironment()
        {
            Map = deployments => from deployment in deployments
                                 select new ReducedResult()
                                 {
                                     Id = deployment.Id,
                                     LastFinishedAt = deployment.FinishedAt,
                                     LastRequestedAt = deployment.RequestedAt,
                                     EnvironmentId = deployment.EnvironmentId,
                                     EnvironmentName = LoadDocument<Environment>(deployment.EnvironmentId).Name,
                                     PackageName = LoadDocument<Package>(deployment.PackageId).NugetId,
                                     PackageId = deployment.PackageId,
                                 };
        }
    }
}